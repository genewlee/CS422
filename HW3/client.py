#! /usr/bin/env python

import socket
import time

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('127.0.0.1', 4220))
message = 'GET /files/TRUST/TRUST.sln HTTP/1.1'
message += ' ' * (2048 - len(message) - len("\r\n"))
#headers = 'dontent-Length:10\r\n'
headers = '\r\nRange:10\r\n'
headers += 'content-type:text/html'
headers += '\r\n' + 'd' * ((100 * 1024) - len(headers) - len(message) - len("\r\n\r\n") - len('\r\n'))
message += headers
message += '\r\n\r\n'
#message += 'foobar'
print(len(message))

#s.sendto(message.encode(), ('127.0.0.1', 4220))
#time.sleep(11);
#body = 'This is the body'
body = "We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux We did not think of the great open plains, the beautiful rolling hills, the winding streams with tangled growth, as ‘wild’. Only to the white man was nature a ‘wilderness’ and only to him was it ‘infested’ with ‘wild’ animals and ‘savage’ people. To us it was tame. Earth was bountiful and we were surrounded with the blessings of the Great Mystery. - Sioux "
message += body
#print (len(body))
s.sendto(message.encode(), ('127.0.0.1', 4220))
reply = s.recv(131072)
print(reply)


'''
GET / HTTP/1.1____________________content-Length:10\r\n
content-type:text/html/r/n/r/n

This is the bodydddddd...
'''

# Two cases that doesnt "work"
# - Single CRLF can be anywhere after 2048 and still valid
# - If total sent is less than doubleThreshold and no double CRLF
#   it's still recognized as valid