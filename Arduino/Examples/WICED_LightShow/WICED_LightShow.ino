
/**
 * An example application for the Adafruit STM32F205 WICED Feather
 * using a Bosch BNO055 IMU and Neopixels connected on pin A1. 
 * 
 * Flashes colours after a gesture is performed.
 */
#include <adafruit_feather.h>
#include <Adafruit_NeoPixel.h>
#include <MLG_GestureRecogniser.h>
#include "MLG_BNO055.h"

#define NEOPIXEL_PIN PA1

// Parameter 1 = number of pixels in strip
// Parameter 2 = Arduino pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
//   NEO_RGBW    Pixels are wired for RGBW bitstream (NeoPixel RGBW products)
Adafruit_NeoPixel strip = Adafruit_NeoPixel(8, NEOPIXEL_PIN, NEO_GRB + NEO_KHZ800);

MLG_GestureRecogniser network(100);
MLG_BNO055 bno = MLG_BNO055();
int ledPin = PA15;
boolean blinker = false;
char* gestureNames[8] = { "Push", "Pull", "ScrewIn", "ScrewOut", "Hit", "Lift", "DragLeft", "DragRight" };

// 14 byte sensor data
struct SensorData {
  // Note: This order is important! These next 14 bytes are 
  // in the same order that the data is in the BNO055 registers.
  // Begin BNO Sensor data
  
  // 8 bytes quaternion data (scaling is 1 / (1 << 14)) as
  // per BNO055 datasheet.
  int16_t quaternion_w;
  int16_t quaternion_x;
  int16_t quaternion_y;
  int16_t quaternion_z;

  // 6 bytes linear acceleration
  int16_t linear_accel_x; // 900 = 1 MSB
  int16_t linear_accel_y; // 900 = 1 MSB
  int16_t linear_accel_z; // 900 = 1 MSB
};

// RGB colour components for each gesture.
uint8_t colour_components[24] = { 
  255, 0, 0,
  127, 127, 0,
  0, 255, 0,
  0, 127, 127,
  0, 0, 255,
  127, 0, 127,
  127, 127, 127,
  200, 40, 0
};

unsigned long lastTime;
SensorData sample;


void setup() {  
  /* Open serial port for debug output. */
  Serial.begin(230400);

  /* Configure feather status LED as an output. */
  pinMode(ledPin, OUTPUT);
  
  while (!bno.begin()) 
  {
    /* There was a problem detecting the BNO055, check wiring. */
    Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");

    /* Blink an error code. */
    blinkError(3);

    /* Wait until device is reset. */
    while (1) {  }
  }
  
  // put your setup code here, to run once:
  pinMode(ledPin, OUTPUT);
  
  strip.begin();
  strip.show(); // Initialize all pixels to 'off'

  lastTime = millis();
}

/* Blinks the feather status LED the specified number of times. 
 * Flashes occur at 2Hz. */
void blinkError(int count) {
  for (int i = 0; i < count; i++) {
      delay(250);
      digitalWrite(ledPin, HIGH);
      delay(250);
      digitalWrite(ledPin, LOW);
  }
}

void loop() {
  unsigned long currentTime = millis();
  
  /* Capture sensor data at 100Hz (every 10ms). */  
  if ((currentTime - 10) >= lastTime) {
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
    bno.readSensorData(MLG_BNO055::BNO055_QUATERNION_DATA_W_LSB_ADDR, (byte*)(&(sample.quaternion_w)), 14);
    runNetwork();
    
    /* Schedule next sample to be taken 10 milliseconds from the last scheduled time. */
    lastTime = lastTime + 10;
  }

  /* Blink at 1Hz, to show the device is operating normally. */
  blinker = (currentTime / 1000) % 2;
  if (blinker) {
    digitalWrite(ledPin, HIGH);
  } else {
    digitalWrite(ledPin, LOW);
  }
}

int stepNumber = 0;
int timeSinceLastGesture = 200;
int lastGestureIndex = 0;
uint32_t noMoveTicks = 0;

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

  int predictedClass = network.getRecognisedGesture();
  if (predictedClass > 0) {
    Serial.println(gestureNames[predictedClass-1]);
    lastGestureIndex = predictedClass - 1;

    timeSinceLastGesture = 200;
  }
  
  if (timeSinceLastGesture > 0) {
    timeSinceLastGesture--;
  }
  if (noMoveTicks < 300) {
    uint32_t red = (colour_components[lastGestureIndex * 3] * timeSinceLastGesture) / 200;
    uint32_t green = (colour_components[lastGestureIndex * 3 + 1] * timeSinceLastGesture) / 200;
    uint32_t blue = (colour_components[lastGestureIndex * 3 + 2] * timeSinceLastGesture) / 200;
    
    uint32_t colour = strip.Color((uint8_t)red, (uint8_t)green, (uint8_t)blue);
    for (int i = 0; i < strip.numPixels(); i++) {
      strip.setPixelColor(i, colour);
    }
    strip.show();
  } else {
    idleLights();
  }
  
  detectIdle(&s);
}

float lastQuaternionW;
float lastQuaternionX;
float lastQuaternionY;
float lastQuaternionZ;

void detectIdle(sensor_sample* s) {
  // Idle detection.
  if (abs(lastQuaternionW - s->quaternionW) < 0.005 &&
    abs(lastQuaternionX - s->quaternionX) < 0.005 &&
    abs(lastQuaternionY - s->quaternionY) < 0.005 &&
    abs(lastQuaternionZ - s->quaternionZ) < 0.005) {
    // If there was no movement, increment counter.
    noMoveTicks++;
  } else {
    noMoveTicks = 0;
  }
  if (noMoveTicks == 200) {
    Serial.println("Idle");
    network.restart();
  }

  lastQuaternionW = s->quaternionW;
  lastQuaternionX = s->quaternionX;
  lastQuaternionY = s->quaternionY;
  lastQuaternionZ = s->quaternionZ;
}

// Show a pattenered effect of white lights to show the system is wokring.
void idleLights() {
  uint32_t lightColour = strip.Color(10, 10, 10);
  uint32_t brightColour = strip.Color(50, 50, 50);
   for (int i = 0; i < strip.numPixels(); i++) {
    if (i % 3 == (noMoveTicks / 10) % 3) {
      strip.setPixelColor(i, brightColour);
    } else {
      strip.setPixelColor(i, lightColour);
    }
  }
  strip.show();
}

