#include "SensorServer.h"
#include <ESPAsyncTCP.h>
#include <ESP8266WiFi.h>

SensorServer::SensorServer(int port):
  _server(port) {
  _readPos = 0;
  _length = 0;
  _transmitPosition = sizeof(struct SensorData);
  _currentClient = NULL;
  _restartRequested = false;
}
    
SensorServer::~SensorServer() {
  close();
}

void SensorServer::begin() {
    Serial.println(sizeof(struct SensorData));
  _server.begin();
  _server.onClient([](void *obj, AsyncClient *client){ ((SensorServer*)(obj))->onClient(client); }, this);
}

void SensorServer::handleClient() {
  if (_currentClient && _currentClient->connected()) {
    size_t bytesToSend = 0;
    size_t bytesSent = 0;
    
    // Check we have transmit buffer space available.
    // Remember that the ESP only has space for a single TCP packet that
    // it must wait for the client to acknowledge before we can send another!
    // Max send size 1460.
    while (_currentClient->canSend() && bytesToSend == bytesSent) {
      // Empty the transmit buffer.
      if (_transmitPosition < sizeof(struct SensorData)) {
        bytesToSend = sizeof(struct SensorData) - _transmitPosition;
        bytesSent = _currentClient->write(((const char*)(&_transmitBuffer)) + _transmitPosition, bytesToSend);
        _transmitPosition += bytesSent;
      }

      // If there is nothing more to send, stop here.
      if (_length == 0) break;

      // Copy the next thing to send into the transmit buffer.
      if (_transmitPosition == sizeof(struct SensorData)) {
        _transmitPosition = 0;
        _transmitBuffer = _sampleBuffer[_readPos];
        _readPos = (_readPos + 1) % SAMPLE_BUFFER_SIZE;
        _length--;
      }
    }
  }
}

void SensorServer::provideSensorData(struct SensorData data) {
  if (_length < SAMPLE_BUFFER_SIZE) {
    unsigned int pos = (_readPos + _length) % SAMPLE_BUFFER_SIZE;
    _sampleBuffer[pos] = data;
    _length++;
  } else {
    // Overwrite the oldest sample (the one we were going to read next.
    _sampleBuffer[_readPos] = data;
    _readPos = (_readPos + 1) % SAMPLE_BUFFER_SIZE;
  }
}

void SensorServer::close() {
  _server.end();
}

void SensorServer::onClient(AsyncClient* client) {
  // Close any existing client.
  if (_currentClient && _currentClient->connected()) {
    _currentClient->close();
  }
  // Clear transmit buffer which may have been filled for previous client.
  _transmitPosition = sizeof(struct SensorData);
    
  // Hook onto events.
  _currentClient = client;
  _currentClient->onDisconnect([](void *obj, AsyncClient *client){ ((SensorServer*)(obj))->onDisconnect(client); }, this);
  _currentClient->onTimeout([](void *obj, AsyncClient* client, uint32_t time){ ((SensorServer*)(obj))->onTimeout(client, time); }, this);
  _currentClient->onData([](void *obj, AsyncClient* client, void* buffer, size_t len) { ((SensorServer*)(obj))->onData(client, buffer, len); }, this);
}

void SensorServer::onDisconnect(AsyncClient* c) {
  // TODO: Maybe check this is the same as the current client. Just following the 
  // example code here.
  c->free();
  delete c;
  _currentClient = NULL;
}

void SensorServer::onTimeout(AsyncClient* client, uint32_t time) {
  // They timed-out. Close the connection.
  client->close();
}

void SensorServer::onData(AsyncClient* client, void* buffer, size_t len) {
  // Only supported command is currently the restart command.
  _restartRequested = true;
}

bool SensorServer::isRestartRequested() {
  return _restartRequested;
}

void SensorServer::clearRestartRequest() {
  _restartRequested = false;
}




