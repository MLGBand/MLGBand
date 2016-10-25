/* TCP Sensor data server written by Team MLG.
 * 
 * */

#ifndef __MLG_SENSOR_SERVER_H__
#define __MLG_SENSOR_SERVER_H__

#include <ESPAsyncTCP.h>

#define SAMPLE_BUFFER_SIZE 250
#define MAX_SAMPLES_TO_SEND (1460 / sizeof(struct SensorData))

// 34 byte sensor data payload
struct SensorData {
  // 4-byte timestamp
  uint32_t timestamp; 

  // Note: This order is important! These next 28 bytes are 
  // in the same order that the data is in the BNO055 registers.
  // Begin BNO Sensor data
  
  // 6 bytes euler data
  int16_t euler_h;
  int16_t euler_r;
  int16_t euler_p;

  // 8 bytes quaternion data
  int16_t quaternion_w;
  int16_t quaternion_x;
  int16_t quaternion_y;
  int16_t quaternion_z;

  // 6 bytes linear acceleration
  int16_t linear_accel_x; // 900 = 1 MSB
  int16_t linear_accel_y; // 900 = 1 MSB
  int16_t linear_accel_z; // 900 = 1 MSB

  // 6-byte gravity vector. For cross-checking only.
  int16_t gravity_x; // 
  int16_t gravity_y; // 
  int16_t gravity_z; // 

  // 1-bytes ambient temperature
  int8_t temperature;
  
  // 1-byte calibration status
  uint8_t calibration;

  // End BNO sensor data.
  
  // 2-bytes battery level
  uint16_t battery_level;

  // The network's predicted class.
  uint8_t predicted_class;

  // The raw probabilities for each gesture.
  // Value from 0 to 10000, where 0 is 0.00%,
  // 10000 is 100.00%.
  float gesture_probabilities[9];
};

// Implementation of Async TCP library based on https://gist.github.com/me-no-dev/116e417ea6a3bbc98b08#file-asynctelnetserver-ino-L2

class SensorServer {
  public:
    SensorServer(int port);
    ~SensorServer();
    void begin();
    void handleClient();
    void provideSensorData(struct SensorData data);
    void close();
    bool isRestartRequested();
    void clearRestartRequest();
  protected:
    void onClient(AsyncClient* client);
    //void onAck(AsyncClient* client, size_t len, uint32_t time);
    void onDisconnect(AsyncClient* c);
    void onTimeout(AsyncClient* client, uint32_t time);
    void onData(AsyncClient* client, void* buffer, size_t len);
    
    // One second of historical sample data at 100 Hz, for buffering purposes.
    // This is going to consume a fairly significant (~4 KB) of our RAM. But it's for a good enough reason. 
    struct SensorData _sampleBuffer[SAMPLE_BUFFER_SIZE];

    // Our transmit buffer. 
    struct SensorData _transmitBuffer;

    // How many bytes of the transmit buffer we've sent.
    int16_t _transmitPosition;
    
    // The index of the next sample to transmit
    unsigned int _readPos;

    // The number of samples available.
    unsigned int _length;

    // Whether the consumer has asked us to reset the LSTM network.
    bool _restartRequested;
    
    AsyncServer _server;
    AsyncClient* _currentClient;
};

#endif // __MLG_SENSORSERVER_H__

