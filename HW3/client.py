#! /usr/bin/env python

import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('127.0.0.1', 4220))
s.send("GET /blah HTP/1.2/r/n")

