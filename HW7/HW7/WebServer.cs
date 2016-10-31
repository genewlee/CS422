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

        private const int FirstLineBreakThreshold = 2048;
        private const int UpToBodyThreshold = (100 * 1024);

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
            int readOffset = 0 , totalRead = 0;
            bool validRequest;
            NetworkStream ns = client.GetStream();

            /* timeout for an individual read on the network stream */
            ns.ReadTimeout = 1500;

            DateTime startTime = DateTime.Now;

            try
            {
                readOffset = ns.Read(buf, 0, buf.Length);
            }
            catch
            {
                return false;
            }

            // Loop to receive all the data sent by the client.
            while (!Encoding.ASCII.GetString(buf).Contains("\r\n\r\n") || readOffset != 0) // read until double line break
            {
                //string test = Encoding.ASCII.GetString(buf);

                /* If the double break has not been received after 10 seconds total */
                if (DateTime.Now - startTime > TimeSpan.FromSeconds(10))
                    return false;

                totalRead += readOffset;
                
                if (!CheckByteArray(buf, totalRead, false)) // do checks in between
                    return false;

                if (!ns.DataAvailable)
                    break;
                
                try
                {
                    readOffset = ns.Read(buf, totalRead, buf.Length - totalRead);
                }
                catch
                {
                    return false;
                }
            }
            //string blah = Encoding.ASCII.GetString(buf);
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

            /*Regex firstCRLFline = new Regex(@"GET /.* HTTP/(\d+\.\d+$)", RegexOptions.IgnoreCase);
            string firstLine = stringOfBuf.Split("\r\n".ToCharArray())[0]; // get the first lin            
            if (totalRead > FirstLineBreakThreshold)
            {
                if (FirstLineBreakThreshold >= firstLine.Length)
                    firstLine = firstLine.Substring(0, firstLine.Length);
                else
                    firstLine = firstLine.Substring(0, FirstLineBreakThreshold);
                if (!firstCRLFline.IsMatch(firstLine)) // is the first line larger than 2048 bytes
                    return false;
            }*/

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

            /* threshold just to reach the first line break.
             * if 2048 or more bytes from the socket and not received the first CRLF line break */
            //Regex singleCRLF = new Regex(@"\r\n$");
            //if (totalRead >= FirstLineBreakThreshold && !stringOfBuf.Contains("\r\n")) //!singleCRLF.IsMatch(stringOfBuf))
            if (totalRead >= FirstLineBreakThreshold && 
                (stringOfBuf.IndexOf("\r\n", StringComparison.CurrentCulture) == stringOfBuf.IndexOf("\r\n\r\n", StringComparison.CurrentCulture)))
            {
                return false;
            }
            /* If in this loop, we haven't hit the body
            * if you have read (100 * 1024) bytes or more and have not received the double line break
            */
            if (totalRead >= UpToBodyThreshold && !stringOfBuf.Contains("\r\n\r\n"))
            {
                return false;
            }

            return true;
        }

        private static WebRequest BuildRequest(TcpClient client)
        {
            byte[] buf = new byte[UpToBodyThreshold];
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
