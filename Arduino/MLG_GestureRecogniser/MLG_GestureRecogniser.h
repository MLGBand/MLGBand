/* 
 * Gesture Recognition API for a wrist-mounted 9DOF sensor.
 *
 * Developed by Team MLG.
 */
#ifndef __MLG_GESTURE_RECOGNISER_H__
#define __MLG_GESTURE_RECOGNISER_H__

// Represents a frame of sensor data.
// The underlying X Y and Z axes should be
// allocated as follows:
// - Z: Heading/Yaw, the direction in which
//      the user's arm is pointing.
// - Y: Roll: the amount the user's arm has
//      rolled. 
// - X: Pitch: If the user's arm is pointing
//      up or down.
// If you are using an external IMU, you
// may able to remap your axes to match this
// scheme in case your sensor mounting does
// not conform to this.
typedef struct {
  // The first component of the quaternion 
  // representing the device orientation.
  float quaternionW;

  // The second component of the quaternion 
  // representing the device orientation.
  float quaternionX;

  // The third component of the quaternion 
  // representing the device orientation.
  float quaternionY;

  // The fourth component of the quaternion 
  // representing the device orientation.
  float quaternionZ;

  // Linear acceleration along X-axis, 
  // in meters/sec^2. Should be the average
  // observed over the sample window.
  float linearAccelerationX;

  // Linear acceleration along Y-axis, 
  // in meters/sec^2. Should be the average
  // observed over the sample window.
  float linearAccelerationY;
  
  // Linear acceleration along Z-axis, 
  // in meters/sec^2. Should be the average
  // observed over the sample window.
  float linearAccelerationZ;
} sensor_sample;

#define GESTURE_COUNT (8)
#define GESTURE_NONE (0)
#define GESTURE_PUSH (1)
#define GESTURE_PULL (2)
#define GESTURE_SCREW_IN (3)
#define GESTURE_SCREW_OUT (4)
#define GESTURE_HIT (5)
#define GESTURE_LIFT (6)
#define GESTURE_DRAG_LEFT (7)
#define GESTURE_DRAG_RIGHT (8)

// Preprocessing helper classes.
class AccelerationIntegralPreprocessor {
  private:
    float _lastSum;
    float _currentSum;
    int _historyPos;
    float _history[50];
    float _integral;

  public:
    AccelerationIntegralPreprocessor();
    ~AccelerationIntegralPreprocessor();
    float getValue(float input);
};

// Number of timesteps (at 10Hz) angle differentials are calculated over.
#define MLG_ANGLE_DIFFERENTIAL_STEPS 6

class AngleDifferentialPreprocessor { 
  private:
    float _history[MLG_ANGLE_DIFFERENTIAL_STEPS];
    int _historyPos;
    bool _initialised;
    
  public:
    AngleDifferentialPreprocessor();
    ~AngleDifferentialPreprocessor();
    float getValue(float input);
};

class LSTMGate;
class LSTMGateState;
class LSTMLayer;
class LSTMLayerState;
class SoftmaxLayer;
class SoftmaxLayerState;

/**
 * Represents a layer of gates for long-short term memory units.
 * There are three such sets of gates per layer:
 *  - Input gates
 *  - Output gates
 *  - Forget gates
 */
class LSTMGate {
  private:
    // LSTM Gate weights
    int _inputSize;
    int _outputSize;

    // Input weights. Size: inputSize * outputSize.
    // Indexed as (output * outputSize + input).
    float* _inputWeights;

    // Temporal weights from last cell outptus. Size: outputSize * outputSize.
    // Indexed as (output * outputSize + input).
    float* _temporalWeights;

    // Weights from current cell states. Size: outputSize.
    // Indexed as (output).
    float* _cellWeights;

    // Bias weights. Size: outputSize.
    // Indexed as (output).
    float* _biases;

  public:
    // Instanciates a new layer of LSTM gates of the given outputSize, 
    // connecting to an input layer of the given inputSize, and 
    // reading its parameters from the given *parameters vector.
    // The position of *parameters will be incremented to the next
    // unused parameter.
    //
    // The parameters will be read in the following order:
    //   - Input weights, indexed as (output * outputSize + input).
    //     Total: outputSize * inputSize
    //   - Temporal weights, indexed as (output * outputSize + input).
    //     Total: outputSize * outputSize
    //   - Cell weights, indexes as (output)
    //     Total: outputSize
    //   - Biases, indexed as (output)
    //     Total: outputSize
    // Grand total: outputSize * (inputSize + outputSize + 2).
    LSTMGate(int inputSize, int outputSize, float** parameters);
    ~LSTMGate();
    void feedForward(LSTMGateState* state);
};

class LSTMGateState {
  public:
    float* _inputs;
    float* _cellStates;
    float* _lastCellOutputs;
    float* _outputs;
    int _inputSize;
    int _outputSize;

    LSTMGateState(int inputSize, int outputSize, float* inputs, float* lastCellOutputs, float* cellStates);
    ~LSTMGateState();
};

/*
 * Respresents a layer of long-short term memory units.
 */
class LSTMLayer {
  private:
    LSTMGate* _inputGate;
    LSTMGate* _outputGate;
    LSTMGate* _forgetGate;
    int _inputSize;
    int _outputSize;

    // Input weights. Size: inputSize * outputSize.
    // Indexed as (output * outputSize + input).
    float* _inputWeights;

    // Temporal weights from last cell outptus. Size: outputSize * outputSize.
    // Indexed as (output * outputSize + input).
    float* _temporalWeights;
    
    // Bias weights. Size: outputSize.
    // Indexed as (output).
    float* _biases;

  public:
    // Instanciates a new layer of LSTM cells of the given outputSize, 
    // connecting to an input layer of the given inputSize, and 
    // reading its parameters from the given *parameters vector.
    // The position of *parameters will be incremented to the next
    // unused parameter.
    //
    // The parameters will be read in the following order:
    //   - Input weights, indexed as (output * outputSize + input).
    //     Total: outputSize * inputSize
    //   - Temporal weights, indexed as (output * outputSize + input).
    //     Total: outputSize * outputSize
    //   - Biases, indexed as (output)
    //     Total: outputSize
    //   - Input, Forget and Output gates (in that order).
    //     Total: outputSize * (inputSize + outputSize + 2) * 3
    // 
    // Grand total: outputSize * (inputSize * 4 + outputSize * 4 + 7)
    LSTMLayer(int inputSize, int outputSize, float** parameters);
    ~LSTMLayer();
    void feedForward(LSTMLayerState* state);
};

class LSTMLayerState {
  public:  
    LSTMGateState* _inputGate;
    LSTMGateState* _forgetGate;
    LSTMGateState* _outputGate;

    int _inputSize;
    int _outputSize;
    float* _inputs;
    
    float* _cellStates;
    
    float* _lastCellOutputs;
    float* _outputs;

    LSTMLayerState(int inputSize, int outputSize, float* inputs, float* outputs);
    ~LSTMLayerState();
};

class SoftmaxLayer {
  public:
    int _inputSize;
    int _outputSize;
    
    // Input weights. Size: inputSize * outputSize.
    // Indexed as (output * outputSize + input).
    float* _inputWeights;
    
    // Bias weights. Size: outputSize.
    // Indexed as (output).
    float* _biases;
    
    // Instanciates a new Softmax Layer of the given outputSize, 
    // connecting to an input layer of the given inputSize, and 
    // reading its parameters from the given *parameters vector.
    // The position of *parameters will be incremented to the next
    // unused parameter.
    //
    // The parameters will be read in the following order:
    //   - Input weights, indexed as (output * outputSize + input).
    //     Total: outputSize * inputSize
    //   - Biases, indexed as (output)
    //     Total: outputSize
    // 
    // Grand total: outputSize * (inputSize + 1)
    SoftmaxLayer(int inputSize, int outputSize, float** parameters);
    ~SoftmaxLayer();
    void feedForward(SoftmaxLayerState* state);
};

class SoftmaxLayerState {
  public:
    int _inputSize;
    int _outputSize;
    float* _inputs;
    float* _outputs;

    SoftmaxLayerState(int inputSize, int outputSize, float* inputs, float* outputs);
    ~SoftmaxLayerState();
};

class LSTMNetwork {
  private:
    int _inputSize;
    int _hiddenSize;
    int _outputSize;
    bool _loadedSuccessfully;
    float* _inputs;
    float* _hiddenStates;
    float* _outputs;
    LSTMLayer* _hiddenLayer;
    LSTMLayerState* _hiddenLayerState;
    SoftmaxLayer* _outputLayer;
    SoftmaxLayerState* _outputLayerState;
    
  public:
    int inputSize();
    int outputSize();
    float* inputs();
    float* outputs();
    void feedForward();
    bool hasLoadedSuccessfully();
    void restart();

    // Instanciates a new LSTM Neural network of the given dimensions and with the
    // given parameters (weights).
    // 
    // The parameters will be read in the following order:
    //   - LSTM Layer
    //     Total: hiddenSize * (inputSize * 4 + hiddenSize * 4 + 7)
    //   - Output (Softmax) Layer
    //     Total: outputSize * (hiddenSize + 1)
    // 
    // Grand total: hiddenSize * (inputSize * 4 + hiddenSize * 4 + 7)
    //            + outputSize * (hiddenSize + 1)
    LSTMNetwork(int inputSize, int hiddenSize, int outputSize, float* parameters);
    ~LSTMNetwork();
};

// Recognises gestures based on orientation sensor data.
// The operating frequency of the recogniser is 10Hz.
// Therefore, sensor data should be provided at 10Hz intervals.
class MLG_GestureRecogniser {
  private:
    LSTMNetwork _network;
    AccelerationIntegralPreprocessor _xIntegral;
    AccelerationIntegralPreprocessor _yIntegral;
    AccelerationIntegralPreprocessor _zIntegral;
    AngleDifferentialPreprocessor _headingDifferential;
    AngleDifferentialPreprocessor _pitchDifferential;
    AngleDifferentialPreprocessor _rollDifferential;
    int _sampleRate;
    int _sampleNumber;
    int _recognisedGesture;
    bool _suppressedGestures[GESTURE_COUNT];
    float _averagingFactor;
    float _xLinearAccelerationAverage;
    float _yLinearAccelerationAverage;
    float _zLinearAccelerationAverage;
    
  public:
    // Initialises a new GestureRecogniser instance,
    // specifying the sample rate (samples per second) input data will
    // be provided at. The specified value will be interpreted as the
    // nearest multiple of ten, so that data can be subsampled down to 10Hz.
    MLG_GestureRecogniser(int samplesPerSecond);

    // Releases resources used by the GestureRecogniser.
    ~MLG_GestureRecogniser();
    
    // Gets the currently configured sample rate (samples per second.)
    int getSampleRate();

    // Processes the provided sensor data frame. This method should
    // be called at frequency of 10Hz (i.e. every 100 milliseconds.)
    // All quantities must be provided, in the units of measure
    // described on sensor_sample.
    void process(sensor_sample* sample);

    // Deterimes whether a gesture been recognised in the last sensor 
    // sample based on the default confidence thresholds.
    bool isGestureRecognised();

    // Returns the number of the gesture that has been recognised
    // using the default confidence thresholds, or zero if no 
    // gesture has been recognised.
    // The value returned by this method will correspond to one of 
    // the GESTURE_* definitions.
    int getRecognisedGesture();

    // Populates the specified array with the raw gesture likelihoods
    // that have been recognised. The array should be equal to the
    // number of gestures recognised plus one.
    // The first element of the array will contain the likelihood
    // (between 0.0 and 1.0) that no gesture was recognised. The
    // subsequent elements will indicate the probability that that
    // gesture number was recognised. 
    // Note: While the returned probabilities are guaranteed to add
    // up to one, these likelihoods may not reflect the true likelihood
    // of that gesture occuring in actual use.
    // This is for fine-tuning recogniser behaviour.
    void getRawGestureLikelihoods(float* values);   

    // Restarts the gesture recognsier as if it has just been created.
    // This will clear all LSTM network state, which may improve accuracy
    // if the recogniser is swapped between users or is put through
    // unusual movements.
    void restart();
};

#endif // __MLG_GESTURE_RECOGNISER_H__
