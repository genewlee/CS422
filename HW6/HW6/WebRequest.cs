using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CS422
{
    public class WebRequest
    {
        NetworkStream _ns;
        ConcurrentDictionary<string, string> _headers;
        string _httpMethod;
        string _requestURI;
        string _httpVersion;
        Stream _body;
        byte[] _buffer;
        long _bufLen;

        public string RequestURI { get { return _requestURI; } }
        public string HTTPMethod { get { return _httpMethod; } }
        public string BodySize { get { 
                long len = GetContentLengthOrDefault();
                if (len != -1)
                    return len.ToString();
                return _bufLen.ToString();
            } 
        }
        public string HTTPVersion { get { return _httpVersion; } }

        public WebRequest(NetworkStream ns, byte[] buf)
        {
            _httpMethod = _requestURI = _httpVersion = string.Empty;
            _ns = ns;
            _buffer = buf;
            _bufLen = -1;
            _headers = new ConcurrentDictionary<string, string>();
            ParseHeaders();
            CreateBodyStream();
        }

        public long GetContentLengthOrDefault()
        {
            long len;
            foreach (var kvp in _headers)
            {
                if (kvp.Key.ToLower() == "content-length")
                {
                    var isLong = Int64.TryParse(kvp.Value, out len);
                    if (isLong)
                        return len;
                }
            }
            return -1;
        }

        private void ParseHeaders()
        {
            string buf = Encoding.ASCII.GetString(_buffer).ToLower();
            string header = buf.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None)[0];       // just the headers portion
            //buf = buf.Substring(0, buf.IndexOf("\r\n\r\n") + 1);

            string[] headers = header.Split("\r\n".ToCharArray()); // split headers by every newline
            RequestInfo(headers[0]);

            foreach (var line in headers)
            {
                if (line.Contains(":"))
                {
                    string[] hv = line.Split(':');
                    _headers[hv[0]] = hv[1];
                }
            }
        }

        private void RequestInfo(string line)
        {
            string[] httpInfo = line.Split(' ');

            _httpMethod = httpInfo[0];
            _requestURI = httpInfo[1];
            _httpVersion = httpInfo[2];
        }

        private void CreateBodyStream()
        {
            // Check if body data in buffer
            string body = Encoding.ASCII.GetString(_buffer).ToLower();
            List<string> bodyPieces = new List<String>(body.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None));       // body portion

            // if not body stream is just the network stream; return;
            if (bodyPieces.Count < 2)
                return;

            // otherwise, need new buffer that starts from the \r\n\r\n
            _bufLen = Encoding.ASCII.GetBytes(bodyPieces[1]).Length;
            byte[] buf = Encoding.ASCII.GetBytes(bodyPieces[1]);;

            if (bodyPieces[1][0] == '\0' && _bufLen > 0)
            {
                for (int i = 0; i < bodyPieces[1].Length; i++)
                {
                    if (bodyPieces[1][i] == '\0')
                    {
                        bodyPieces[1] = bodyPieces[1].Remove(i, 1);
                        i--;
                    }
                }
                _bufLen = Encoding.ASCII.GetBytes(bodyPieces[1]).Length;
                if (_bufLen == 0)
                {
                    bodyPieces.RemoveAt(1);
                }
                if (bodyPieces.Count > 1)
                {
                    buf = Encoding.ASCII.GetBytes(bodyPieces[1]);
                }
                //string test = Encoding.ASCII.GetString(buf);
            }

            if (bodyPieces.Count == 1)
            {
                _body = _ns;
                //if (_bufLen != -1)
                    //_body.SetLength(_bufLen);
            }
            else
            {
                // create memory stream of buf
                MemoryStream ms = new MemoryStream(buf);

                // create concat stream of the memory stream and network stream
                if (_bufLen < 0)
                    _body = new ConcatStream(ms, _ns);
                else
                    _body = new ConcatStream(ms, _ns, _bufLen);
            }
        }

        public void WriteNotFoundResponse(string pageHTML)
        {
            string template = "HTTP/1.1 404 NOT FOUND\r\n" +
                "Content-Type: text/html\r\n" +
                "Content-Length: {0}\r\n" + "\r\n" +
                "{1}";
            string response = string.Format(template, Encoding.ASCII.GetBytes(pageHTML).Length, pageHTML);

            byte[] byteResponse = Encoding.ASCII.GetBytes(response);
            _ns.Write(byteResponse, 0, byteResponse.Length);
        }

        public bool WriteHTMLResponse(string htmlString)
        {
            string template = "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html\r\n" +
                "Content-Length: {0}\r\n" + "\r\n" +
                "{1}";
            string response = string.Format(template, Encoding.ASCII.GetBytes(htmlString).Length, htmlString);

            byte[] byteResponse = Encoding.ASCII.GetBytes(response);
            _ns.Write(byteResponse, 0, byteResponse.Length);
            return true;
        }
    }
}

