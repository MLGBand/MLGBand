#!/usr/bin/env python

import socket
from struct import *

# TCP_IP = '127.0.0.1'
# TCP_PORT = 80
# BUFFER_SIZE = 46
#MESSAGE = "Hello, World!"

class DeviceConnection:
    #TCP_IP = '127.0.0.1'
    # TCP_PORT = 80
    # BUFFER_SIZE = 46
    # RECEIVE_BUFFER = []
    
    def __init__(self, s=None):
        if s is None:
            self.s = socket.socket(
                            socket.AF_INET, socket.SOCK_STREAM)
            self.tcp_port = 80
            # self.tcp_port = 5005
            self.buffer_size = 72
        else:
            self.s = s
    
    def Connect(self, ip_address):
        # print "Connecting..."
        self.s.connect((ip_address, self.tcp_port))
        
    def Read(self):
        # print "Trying to read data..."
         
        # # data = unpack('!IhhhhhhhhhhhhhbBHB', self.s.recv(self.buffer_size))
        # data = self.s.recv(self.buffer_size)
        # # print len(data)
        chunks = []
        bytes_recd = 0
        while bytes_recd < self.buffer_size:
            # if bytes_recd
            chunk = self.s.recv(self.buffer_size - bytes_recd)
            # print len(chunk)
            if len(chunk) == 1:
                # print 'continuing'
                continue
            if chunk == b'':
                raise RuntimeError("socket connection broken")
            chunks.append(chunk)
            bytes_recd = bytes_recd + len(chunk)
                # print "Length 1 chunk:", chunk
        chunks_recvd = b''.join(chunks)
        # print len(chunks_recvd)
        data = unpack('<IhhhhhhhhhhhhhbBHBBfffffffff', chunks_recvd)
        return data
    
    def Calibrate(self): 
        toSend = 0
        data = pack("<B", toSend)
        self.s.send(data)
    def Disconnect(self):
        # print "Disconnecting..."
        self.s.close()
     
    # Event methods for API   
    def gestureProbabilities(self):
        pass
    
    def gestureOccured(self):
        pass
        
    def disconnected(self):
        pass
    
    def connected(self):
        pass
    # Where to connect to for API
    def connectTo(ipaddress):
        pass
        
        
# device = DeviceConnection()
# # device.Connect("192.168.4.1")
# device.Connect("127.0.0.1")
# data = ""
# i = 0
# while data != None:
#     # data = device.Read()
#     # print data
#
#     if i%100 == 0:
#         data = device.Read()
#         print data
#         '''
#         relax = 0
#         push = 1
#         pull = 2
#         Screwin = 3
#         Screwout = 4
#         hit = 5
#         lift = 6
#         dragleft = 7
#         dragright = 8
#         '''
#     i += 1
# device.Disconnect