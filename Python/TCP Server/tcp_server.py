#!/usr/bin/env python

import socket
from struct import *
from random import randint, uniform


TCP_IP = '127.0.0.1'
TCP_PORT = 5005
BUFFER_SIZE = 36  # Normally 1024, but we want fast response

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)

conn, addr = s.accept()
print 'Connection address:', addr
packet_num = 0
random_gesture = 0
while 1:
    if packet_num%1 == 0: ## Only change the gesture every 1000 packets
        random_gesture = randint(0,8)
    if packet_num%3 == 0:
        random_gesture = 0 #favour resting state
    p1 = uniform(0,1)
    p2 = uniform(0,1)
    p3 = uniform(0,1)
    p4 = uniform(0,1)
    p5 = uniform(0,1)
    p6 = uniform(0,1)
    p7 = uniform(0,1)
    p8 = uniform(0,1)
    p9 = uniform(0,1)
    unpacked_data = range(36)
    unpacked_data[0] = packet_num
    data = pack("<IhhhhhhhhhhhhhbBHBBfffffffff", packet_num, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,random_gesture, 0, p1, p2, p3, p4, p5, p6, p7,p8,p9)
    print "sending:", data
    conn.send(data) 
    packet_num += 1
conn.close()