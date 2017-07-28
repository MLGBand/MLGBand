# DECO3801 Team MLG - GRAPI: Gesture Recognition API

## Overview
The **Gesture Recognition API** provides gesture recognition functionality for wrist-mounted embedded devices.

It uses a neural network to detect which gesture is being performed, and outputs probability values to the host device (e.g. desktop computer, mobile). This guide is split into three parts. The first covers the gesture recognition API. The second covers the demo application. The third is a code guide of what can be found in this git repository.

See our semi-awful video demonstration:
https://www.youtube.com/watch?v=5C_GULdiQrM

## Gesture API

### Hardware Requirements

The gesture API is intended to be run on Arduino systems. The system used must have a 9-DOF (Magnetometer, accelerometer and gyroscope) sensor that is capable of supplying data at 10Hz. For use with the demo app, the hardware must also be capable of transmission over 802.11 wifi. 

Note that the IMU used must be configured with XYZ axis as follows:

- Z: Heading/Yaw, the direction in which the user's arm is pointing.
- Y: Roll: the amount the user's arm has rolled. 
- X: Pitch: If the user's arm is pointing up or down.

For the remainder of this guide, including all examples, the hardware used was:

- an ESP8266 microcontroller flashed with Arduino
- A Bosch BNO055 absolute orientation sensor connected via I2C


### Installation

The complete software for the API can be cloned from this git repository or downloaded as a zip file. Note that the drivers for the hardware must be installed prior to porting to the device. Drivers for the ESP8266 can be sourced from [silicon labs](https://www.silabs.com/products/mcu/Pages/USBtoUARTBridgeVCPDrivers.aspx). The C++ MLG_GestureRecogniser file can then be ported to the device. The easiest way to do this is through the Arduino IDE which can be found here. This is compatible for Windows, Mac or Linux.

### Usage

The following code demonstrates the usage of the API.

```c++ 
#include "MLG_GestureRecogniser.h"

MLG_GestureRecogniser grapi(100); // initialise new gesture recogniser at 100Hz (matches our Bosch BNO055 sensor)
	
int sample_rate = graphi.getSampleRate(); // will return 100 samples per second

// Arduino sketch main loop.
void loop() {
// Every 10ms (or whenever the sensor has a sample.)
	if (isSensorDataAvailable()) {
		recogniseGestures();
	}
}

void recogniseGestures() {
// Set sensor_sample data (note that this should be done from your IMU)
sensor_sample ss; // initialise new sensor_sample struct
ss->quaternionW = 1.5f;
ss->quaternionX = 1.5f;
ss->quaternionY = 1.5f;
ss->quaternionZ = 1.5f;
ss->linearAccelerationX = 2.3f; // in meters per second per second
ss->linearAccelerationY = 0f;
ss->linearAccelerationZ = 1.9f;

// Process the sample data
grapi.process(&ss);

if(graphi.isGestureRecognised()){ // true if the processed data indicated a gesture based on confidence thresholds
	int recognised_gesture = graphi.getRecognisedGesture();
	// 0 = relax, 1 = push, 2 = pull, 3 = twist in, 4 = twist out, 5 = hit, 6 = lift, 7 = drag left, 8 = drag right
	// Respond to gesture as you like.
}

float values[9];
getRawGestureLikelihoods(&values) // values now contains the raw probabilities of each gesture occurring
}
```

### Gestures Detected

The following gestures are supported by the API, in addition to a resting state.

- Push
- Pull
- Twist In
- Twist Out
- Hit
- Lift
- Drag Left
- Drag Right

## Demo App

### Dependencies

The demo app is written in Python 3 and requires the following dependencies. Click the links to see installation instructions. Note that this will not run on Python 2.7.

- [Python 3](https://www.python.org/download/releases/3.0/)
- [wxPython Phoenix](https://wiki.wxpython.org/ProjectPhoenix)
- [PyOpenGL](http://pyopengl.sourceforge.net/)
- [Python Imaging Library](http://www.pythonware.com/products/pil/)

Alternatively, on Unix systems, the following commands can be run to set up all dependencies within a virtual environment (assuming pip is already installed).

```
sudo pip install virtualenv
pyvenv venv
source venv/bin/activate
sudo pip install -r requirements.txt
sudo pip install --upgrade --trusted-host wxpython.org --pre -f 
http://wxpython.org/Phoenix/snapshot-builds/ wxPython_Phoenix
deactivate
```


### Installation

Download or clone the folder in the git repository. Unzip if zipped. No installation necessary as long as above dependencies are supplied.

### Usage

In the terminal/console, run: 

```python3 gui.py```

At this point enter in an IP address to connect to (when computer has been connected to wireless access point of device), and click connect. The app should then receive gesture data and output the gestures as a visual representation.


## Code Outline

The codebase is structured into the following high-level components, described in table below.

<table>
	<tr>
		<th> Directory </th>
		<th> Description </th>
		<th> Language(s) </th>
	</tr>
	<tr>
		<td> Arduino/ </td>
		<td> All the software the runs on the Arduino. Contains our Gesture Recognition API and examples.
		</td>
		<td> C++ </td>
	</tr>
		<tr>
		<td> Demo/ </td>
		<td> Contains the demo GUI and dependencies. Allows for connection and graphical gesture representation.
		</td>
		<td> Python </td>
	</tr>
		<tr>
		<td> GestureAdapter/ </td>
		<td> A client-side application that connects to the Arduino over WiFi and logs the raw sensor data it receives.
		</td>
		<td> C# </td>
	</tr>
		<tr>
		<td> NNWorkspace/ </td>
		<td> Our Long-Short Term Memory network (LSTM) training infrastructure. Used to create models from raw data.
		</td>
		<td> C# </td>
	</tr>
		<tr>
		<td> Matlab/ </td>
		<td> MATLAB data conversion scripts, experiments and our early feed-forward neural networks.
		</td>
		<td> MATLAB </td>
	</tr>
		<tr>
		<td> Website/ </td>
		<td> The launch website for the Minimum Viable Product.
		</td>
		<td> PHP, HTML5, CSS3, JS, SQL </td>
	</tr>	
</table>

Key sub-directories are described in the table below.

<table>
	<tr>
		<th> Sub-directory </th>
		<th> Description </th>
	</tr>
	<tr>
		<td> Arduino/Examples/DataCollection </td>
		<td> An example application that provides sensor data and gesture probabilities over TCP, to facilitate data collection and/or the demo interface. </td>
	</tr>
		<tr>
		<td> Arduino/Examples/libraries </td>
		<td> Contains the asynchronous TCP library we use in our DataCollection example. </td>
	</tr>
		<tr>
		<td> Arduino/MLG_GestureRecogniser </td>
		<td> The packaged Arduino library/API and metadata.  </td>
	</tr>
		<tr>
		<td> NNWorkspace/NNWorkbench/
NeuralNetworks </td>
		<td> Implementation of various Neural Network layers, including LSTM, plain recurrent, feed-forward and softmax layers.</td>
	</tr>
		<tr>
		<td> NNWorkspace/NNWorkbench/
Preprocessing </td>
		<td> Contains a cloned version of on-device preprocessing and other raw data preprocessing used to control Neural Network overfitting and facilitate training. </td>
	</tr>

</table>

## Improving and extending the gesture set

Neural Networks can be trained and exported using the NNWorkbench tool.

The data used to train the published network is available for you to use here:
https://drive.google.com/drive/u/0/folders/0BxgQ4LM2qgB9c0xjNUZfX3EwNGc

You can collect your own data using the DataCollection app provided in the Arduino/Examples folder.
