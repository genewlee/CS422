using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.IO;

namespace CS422
{
    public class WebRequest
    {
        NetworkStream ns;
        ConcurrentDictionary<string, string> headers;
        string httpMethod;
        string requestTarget;
        string httpVersion;
        Stream body;

        public NetworkStream networkStream
        {
            set { ns = value; }
        }

        public ConcurrentDictionary<string, string> Headers
        {
            set { headers = value; }
        }

        public string HTTPMethod
        {
            set { httpMethod = value; }
        }

        public string RequestTarget
        {
            set { requestTarget = value; }
        }

        public string HTTPVersion
        {
            set { httpVersion = value; }
        }

        public Stream Body
        {
            set { body = value; }
        }

        public WebRequest() { }

        public WebRequest(NetworkStream NS, ConcurrentDictionary<string, string> Headers,
                          string HTTPMethod, string RequestTarget, string HTTPVersion, Stream Body)
        {
            ns = NS;
            headers = Headers;
            httpMethod = HTTPMethod;
            requestTarget = RequestTarget;
            httpMethod = HTTPMethod;
            body = Body;
        }

        public long GetContentLengthOrDefault()
        {
            long len;
            foreach (var kvp in headers)
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

        public void WriteNotFoundResponse(string pageHTML)
        {
            
        }

        public bool WriteHTMLResponse(string htmlString)
        {

        }
    }
}

