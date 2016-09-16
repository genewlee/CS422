using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CS422
{
    public class WebServer
    {

        private const string DefaultTemplate = "HTTP/1.1 200 OK\r\n" +
                                       "Content-Type: text/html\r\n" + "\r\n\r\n" +
                                       "<html>ID Number: {0}<br>" + "DateTime.Now: {1}<br>" + "Requested URL: {2}</html>";

        public static bool Start(int port, string responseTemplate)
        {
            byte[] buf = new byte[4096];

            TcpListener listener = new TcpListener(IPAddress.Any, port); 
            listener.Start();
            TcpClient client;

            while (true)
            {
                // blocking call to accept client
                try
                {
                    client = listener.AcceptTcpClient();
                }
                catch
                {
                    return false;
                }

                bool validResponse = ReadRequestFromClient(client, ref buf);

                if (validResponse)
                {
                    WriteResponseToClient(client, buf);
                    client.GetStream().Dispose();
                    client.Close();
                    return true;
                }
                client.GetStream().Dispose();
                client.Close();
                return false;
            }
        }

        /// <summary>
        /// Reads the request from client.
        /// </summary>
        public static bool ReadRequestFromClient(TcpClient client, ref byte[] buf)
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

        /// <summary>
        /// Writes the response to clients network stream
        /// Splits the buf and gets the url
        /// </summary>
        public static void WriteResponseToClient(TcpClient client, byte[] buf)
        {
            // / get the string in between the the 4th character which comes after GET and the next whitespace
            string _url = Encoding.ASCII.GetString(buf).Substring(4).Split(' ')[0];
            string response = string.Format(DefaultTemplate, 11216720, DateTime.Now, _url);
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);

            if (client.GetStream().CanWrite)
            {
                client.GetStream().Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}

