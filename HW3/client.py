#! /usr/bin/env python

import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('127.0.0.1', 4220))
s.send("GE0 /blah HTTP/1.2/r/n")

