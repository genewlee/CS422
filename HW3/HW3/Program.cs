using System;
namespace CS422
{
    public class Program
    {
        private const string DefaultTemplate = "HTTP/1.1 200 OK\r\n" +
                                    "Content-Type: text/html\r\n" + "\r\n\r\n" +
                                    "<html>ID Number: {0}<br>" + "DateTime.Now: {1}<br>" + "Requested URL: {2}</html>";
        static void Main()
        {
            //var blah = string.Format(DefaultTemplate, 11216720, DateTime.Now, "blah");
            //Console.WriteLine(blah);

            WebServer.Start(4220, DefaultTemplate);

            /*System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 4220);
            listener.Start();
            System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
            var buf = System.Text.Encoding.ASCII.GetBytes(blah);

            int i_read, totalRead = 0;

            // Loop to receive all the data sent by the client.
            i_read = client.GetStream().Read(buf, 0, buf.Length);

            while (i_read != 0)
            {
                totalRead += i_read;
                i_read = client.GetStream().Read(buf, totalRead, buf.Length - totalRead);
            }

            //client.GetStream().Read(buf, 0, buf.Length);
            //buf = System.Text.Encoding.ASCII.GetBytes(blah);
            client.GetStream().Write(buf, 0, buf.Length);
            client.Close();*/
        }
    }
}

