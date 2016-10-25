/* 
 * Creates a WiFi access point and create a TCP server to stream BNO055 sensor data. 
 * 
 * Developed by Team MLG, based on WiFi AP example provided with ESP8266. 
 *  
 * The following copyright/licencing notices apply: 
 * ==================
 * Copyright (c) 2015, Majenko Technologies
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * 
 * * Redistributions in binary form must reproduce the above copyright notice, this
 *   list of conditions and the following disclaimer in the documentation and/or
 *   other materials provided with the distribution.
 * 
 * * Neither the name of Majenko Technologies nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#include <ESP8266WiFi.h>
#include <WiFiServer.h>
#include "MLG_GestureRecogniser.h"
#include "MLG_BNO055.h"
#include "SensorServer.h"

/* Set these to your desired credentials. */
const char *ssid = "Team MLG's Gesture Band";
const char *password = "gestureband";
SensorServer server(80);

char* gestureNames[8] = { "Push", "Pull", "ScrewIn", "ScrewOut", "Hit", "Lift", "DragLeft", "DragRight" };

MLG_GestureRecogniser network(100);
MLG_BNO055 bno = MLG_BNO055();
boolean blinker = false;

void setup() {
  /* Open serial port for debug output. */
  Serial.begin(230400);
  Serial.println();

  /* Configure feather status LED as an output. */
  pinMode(0, OUTPUT);
  
  while (!bno.begin()) 
  {
    /* There was a problem detecting the BNO055, check wiring. */
    Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");

    /* Blink an error code. */
    blinkError(3);

    /* Wait until device is reset. */
    while (1) { yield(); }
  }

  /* Use external reference oscillator. */
  bno.setExtCrystalUse(true);

  /* Setup WiFi access. */
	Serial.print("Configuring access point...");
	WiFi.softAP(ssid, password);

	IPAddress myIP = WiFi.softAPIP();
	Serial.print("AP IP address: ");
	Serial.println(myIP);

  /* Start TCP server. */
  server.begin();
	Serial.println("TCP server started");    
}

/* Blinks the feather status LED the specified number of times. 
 * Flashes occur at 2Hz. */
void blinkError(int count) {
  for (int i = 0; i < count; i++) {
      delay(250);
      digitalWrite(0, HIGH);
      delay(250);
      digitalWrite(0, LOW);
  }
}

unsigned long lastTime = millis();
SensorData sample;
  
void loop() {
  unsigned long currentTime = millis();
  
  /* Capture sensor data at 100Hz (every 10ms). */  
  if ((currentTime - 10) >= lastTime) {
    /* Record actual timestamp of sampling, milliseconds since device powered on. */
    sample.timestamp = millis();  

    /* Capture 28 bytes of raw sensor data, starting at the Euler Heading LSB address.
     * This provides for (refer to BNO055 datasheet): 
     *   - Euler Heading, Roll, Pitch data. 
     *   - Quaternion W, X, Y, Z data.
     *   - Linear Acceleration X, Y, Z data.
     *   - Gravity X, Y, Z data.
     *   - Device temperature (1 byte).
     *   - Calibration status bits (1 byte).
     * Note that each of these quantities, except for temperature and calibration status 
     * is a two-byte word with LSB and MSB components.
     */
    bno.readSensorData(MLG_BNO055::BNO055_EULER_H_LSB_ADDR, (byte*)(&(sample.euler_h)), 28);
    
    runNetwork();
    
    /* Provide sensor data to TCP server. */
    server.provideSensorData(sample);

    if (server.isRestartRequested()) {
      /* Reset the LSTM network if requested to do so. */
      Serial.println("Restarting");
      network.restart();
      server.clearRestartRequest();
    }
    
    /* Schedule next sample to be taken 10 milliseconds from the last scheduled time. */
    lastTime = lastTime + 10;
  }

  /* Stream data to clients. */
  server.handleClient();

  /* Blink at 1Hz, to show the device is operating normally. */
  blinker = (currentTime / 1000) % 2;
  if (blinker) {
    digitalWrite(0, HIGH);
  } else {
    digitalWrite(0, LOW);
  }

  /* Yield to ESP OS, so it can run the WiFi stack and everything else. */
  yield();
}

int stepNumber = 0;

void runNetwork() {
  sensor_sample s;
  
  float quaternionScale = 1.0f / (1 << 14); // As per Bosch BNO055 datasheet.
  s.quaternionW = sample.quaternion_w * quaternionScale;
  s.quaternionX = sample.quaternion_x * quaternionScale;
  s.quaternionY = sample.quaternion_y * quaternionScale;
  s.quaternionZ = sample.quaternion_z * quaternionScale;
  s.linearAccelerationX = sample.linear_accel_x / 100.0f;
  s.linearAccelerationY = sample.linear_accel_y / 100.0f;
  s.linearAccelerationZ = sample.linear_accel_z / 100.0f;
  network.process(&s);
  
  sample.predicted_class = network.getRecognisedGesture();

  float raw_probabilities[9];
  network.getRawGestureLikelihoods(raw_probabilities);
  
  for (int i = 0; i < 9; i++) {
    sample.gesture_probabilities[i] = raw_probabilities[i];
  }
  
  if (sample.predicted_class > 0) {
    Serial.println(gestureNames[sample.predicted_class-1]);
  }
}

