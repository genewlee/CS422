using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;

namespace CS422
{
    public static class WebServer
    {
        private static readonly HashSet<WebService> _services = new HashSet<WebService> { new DemoService() };
        private static readonly BlockingCollection<TcpClient> _collection = new BlockingCollection<TcpClient>();
        private static readonly List<Thread> _threadPool = new List<Thread>();
        private static Thread _listeningThread;
        private static TcpListener _listener;
        private static int _nThreads;
        private static int _port;

        /*
         * Accept new TCP socket connection
         * Get a thread from the thread pool and pass it the TCP socket 
         * Repeat
         */
        public static void Start(Int32 port, Int32 nThreads)
        {
            if (_listeningThread != null)
            {
                throw new InvalidOperationException("Server already started.");
            }

            // If this value is less than or equal to zero, use 64 as a default instead
            _nThreads = nThreads < 0 ? 64 : nThreads;

            _port = port;

            _listeningThread = new Thread(() =>
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();

                while (true)
                {
                    try
                    {
                        var client = _listener.AcceptTcpClient();
                        _collection.Add(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            });
                                          
            _listeningThread.Start();

            // Initialize all threads to await a client connection.
            for (int i = 0; i < _nThreads; i++)
            {
                Thread t = new Thread(ThreadWork);
                t.Start();
                _threadPool.Add(t);
            }
        }

        /*
        * ThreadWork function to serve as the request-handling function for a thread from the thread pool
        */
        private static void ThreadWork()
        {
            while (true)
            {
                TcpClient client = _collection.Take();
                if (client == null)
                    break;

                /*
                 * Processing a client involves calling the BuildRequest function first. 
                 * Should the return value be null, the client connection is closed immediately 
                 * and the thread goes back into a waiting/sleeping state.
                 * Should it be non-null, 
                 * then an appropriate handler is found in the list of handlers/services that the web
                 * server stores, and the request is sent to that handler by calling its “Handle” function
                 */
                WebRequest request = BuildRequest(client);

                if (request == null)
                    client.Close();
                else
                {
                    WebService handler = Handler(request);

                    if (handler != null)
                    {
                        handler.Handler(request);
                        client.GetStream().Dispose();
                        client.Close();
                    }
                    else
                    {
                        request.WriteNotFoundResponse(request.RequestURI);
                        client.GetStream().Dispose();
                        client.Close();
                    }
                }
            }
        }

        /*
         * The service “Handle” function is expected to write a response using the request object. 
         * Should no such handler exist, write a 404 – Not Found response. Close the client connection 
         * and dispose the NetworkStream and TcpClient after either of these scenarios 
         * (i.e. either handling with an appropriate handler or writing a 404).
         */
        private static WebService Handler(WebRequest request)
        {
            foreach (var service in _services)
            {
                if (string.Compare(request.RequestURI, service.ServiceURI, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return service;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the request from client.
        /// </summary>
        private static bool ReadRequestFromClient(TcpClient client, ref byte[] buf)
        {
            int readOffset, totalRead = 0;
            bool validRequest;
            NetworkStream ns = client.GetStream();

            // Loop to receive all the data sent by the client.
            readOffset = ns.Read(buf, 0, buf.Length);

            while (readOffset != 0)
            {
                totalRead += readOffset;
                if (!CheckByteArray(buf, totalRead, false)) // do checks in between 
                {
                    return false;
                }
                if (!ns.DataAvailable) // Read() blocks until there are bytes available for reading
                {
                    break;
                }
                readOffset = ns.Read(buf, totalRead, buf.Length - totalRead);
            }
            validRequest = CheckByteArray(buf, totalRead, true); // do final check after all has been read
            return validRequest;
        }

        /// <summary>
        /// Checks the byte array for valid request (ie GET and HTTP version)
        /// </summary>
        public static bool CheckByteArray(byte[] buf, int totalRead, bool readAll)
        {
            Regex regex = new Regex(@"HTTP/(\d+\.\d+)", RegexOptions.IgnoreCase);
            string stringOfBuf = Encoding.ASCII.GetString(buf);

            // If the entire stream has been read but it's total bytes read is less than 4
            // If the total bytes read is >=4 and does equal to "GET "
            if ((totalRead < 4 && readAll) || Encoding.ASCII.GetString(buf, 0, totalRead < 4 ? totalRead : 4) != "GET ")
            {
                return false;
            }
            if (readAll && !regex.IsMatch(stringOfBuf))
            {// If all has been read but the regex of HTTP does not match -> This may be never be handled because of next if statement
                return false;
            }
            else if (regex.IsMatch(stringOfBuf))
            {// If there is a match of HTTP/d.d then check if it is the correct version of 1.1
                Regex ver = new Regex(@"\d+\.\d+");
                string http = regex.Match(stringOfBuf).ToString();
                string version = ver.Match(http).ToString();
                if (version != "1.1")
                    return false;
            }
            return true;
        }

        private static WebRequest BuildRequest(TcpClient client)
        {
            byte[] buf = new byte[4096];
            bool valid = ReadRequestFromClient(client, ref buf);

            NetworkStream ns = client.GetStream();
            if (!valid)
            {
                ns.Dispose();                   // close networkstream
                client.Close();                 // close client
                return null;
            }
            WebRequest request = new WebRequest(ns, buf);

            return request;
        }

        public static void AddService(WebService service)
        {
            _services.Add(service);
        }

        public static void Stop ()
        {
            _listener.Stop();
            _listeningThread.Join();


            for (int i = 0; i < _nThreads; i++)
            {
                _collection.Add(null);
            }

            foreach (Thread thread in _threadPool)
            {
                thread.Join();
            }
        }
    }
}
