using System;
using System.Text;
using System.IO;

namespace CS422
{
    public class FilesWebService : WebService
    {
        private FileSys422 m_fs;

        public FilesWebService(FileSys422 fs) 
        {
            m_fs = fs;
        }

        public override string ServiceURI
        {
            get
            {
                return "/files";
            }
        }

        public override void Handler(WebRequest req)
        {
            if (!req.RequestURI.StartsWith(ServiceURI, StringComparison.CurrentCulture))
            {
                throw new InvalidOperationException();
            }

            if (req.RequestURI == "/files" || req.RequestURI == "/files/")
            {
                RespondWithList(m_fs.GetRoot(), req);
                return;
            }

            string[] pieces = req.RequestURI.Substring(ServiceURI.Length).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pieces == null || pieces.Length == 0)
            {
                req.WriteNotFoundResponse("404: Path error.");
                return;
            }
            Dir422 dir = m_fs.GetRoot();
            for (int i = 0; i < pieces.Length - 1; i++)
            {
                string piece = pieces[i];
                dir = dir.GetDir(piece);
                if (dir == null)
                {
                    req.WriteNotFoundResponse("404: Path error.");
                    return;
                }
            }

            string name = PercentDecoding(pieces[pieces.Length - 1]);

            File422 file = dir.GetFile(name);
            if (file != null)
            {
                RespondWithFile(file, req);
                return;
            }

            dir = dir.GetDir(name);
            if (dir == null)
            {
                req.WriteNotFoundResponse("404: Path error.");
                return;
            }
            RespondWithList(dir, req);
        }

        private void RespondWithList(Dir422 dir, WebRequest req)
        {
            string html = BuildDirHTML(dir);
            req.WriteHTMLResponse(html);
        }

        private string BuildDirHTML(Dir422 directory)
        {
            StringBuilder html = new StringBuilder("");

            //Files
            String dirPath = "";
            Dir422 temp = directory;
            while (temp.Parent != null)
            {
                dirPath = temp.Name + "/" + dirPath;
                temp = temp.Parent;
            }
            dirPath = ServiceURI + "/" + dirPath;

            if (directory.GetFiles().Count > 0)
                html.AppendLine("<h1 style=\"color:black;\">Files</h1>");
            foreach (File422 file in directory.GetFiles())
            {
                //string name = PercentEncoding(file.Name);
                //string href = dirPath + name;
                string href = dirPath + file.Name;
                html.AppendFormat("<a href=\"{0}\">{1}</a>   <br>", href, file.Name);
            }

            //Directories
            //General Note: Don't forget percent encoding and decoding.
            if (directory.GetDirs().Count > 0)
                html.AppendLine("<hr></hr><h1 style=\"color:black;\">Folders</h1>");
            foreach (Dir422 d in directory.GetDirs())
            {
                //string name = PercentEncoding(d.Name);
                //string href = dirPath + name;
                string href = dirPath + d.Name;
                html.AppendFormat("<a href=\"{0}\">{1}</a>   <br>", href, d.Name);
            }
            return html.ToString();
        }

        private void RespondWithFile(File422 file, WebRequest req)
        {
            Stream fs = file.OpenReadOnly();

            if (req.Headers.ContainsKey("range"))
            {
                WriteWithRangeHeader(file, fs, req);
                return;
            }

            // Send response code and headers
            string response = "HTTP/1.1 200 OK\r\n" +
                "Content-Length: " + fs.Length + "\r\n" +
                "Content-Type: " + file.GetContentType() + "\r\n\r\n";
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            req.WriteResponse(responseBytes, 0, responseBytes.Length);
                
            while (true)
            {
                byte[] buf = new byte[4096];
                int read = fs.Read(buf, 0, buf.Length);
                if (read == 0) { break; }
                req.WriteResponse(buf, 0, read);
            }
            fs.Close();
        }

        /// <summary>
        /// Writes the with range header.
        /// </summary>
        private void WriteWithRangeHeader(File422 file, Stream fs, WebRequest req)
        {
            String[] ranges = req.Headers["range"].Trim().Substring("byte= ".Length).Split('-');

            long from;
            Int64.TryParse(ranges[0], out from);
            if (fs.Length <= from)
            {
                string invalid = "HTTP/1.1 416 Requested Range Not Satisfiable\r\n\r\n";
                byte[] invalidResponseBytes = Encoding.ASCII.GetBytes(invalid);
                req.WriteResponse(invalidResponseBytes, 0, invalidResponseBytes.Length);
            }

            long to;
            if (ranges[1] != string.Empty)
                Int64.TryParse(ranges[1], out to);
            else
                to = fs.Length - 1;

            // Send response code and headers
            string response = "HTTP/1.1 206 Partial content\r\n" +
                "Content-Range: bytes " + from.ToString() + "-" + to.ToString() + "/" + fs.Length.ToString() + "\r\n" +
                "Content-Length: " + (to + 1 - from).ToString() + "\r\n" +
                "Content-Type: " + file.GetContentType() + "\r\n\r\n";
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            req.WriteResponse(responseBytes, 0, responseBytes.Length);


            fs.Seek(from, SeekOrigin.Begin);
            while (true)
            {
                byte[] buf = new byte[4096];
                int read = 0;
                read = fs.Read(buf, 0, buf.Length);
                if (read == 0) { break; }
                req.WriteResponse(buf, 0, read);
                if (fs.Position >= to) { break; }
            }
            fs.Close();
        }

        private string PercentDecoding(string name)
        {
            string s = name;

            if (s.Contains("%20")) s = s.Replace("%20", " ");
            if (s.Contains("%22")) s = s.Replace("%22", "\"");
            if (s.Contains("%25")) s = s.Replace("%25", "%");
            if (s.Contains("%2D")) s = s.Replace("%2D", "-");
            if (s.Contains("%2E")) s = s.Replace("%2E", ".");
            if (s.Contains("%3C")) s = s.Replace("%3C", "<");
            if (s.Contains("%3E")) s = s.Replace("%3E", ">");
            if (s.Contains("%5C")) s = s.Replace("%5C", "\\");
            if (s.Contains("%5E")) s = s.Replace("%5E", "^");
            if (s.Contains("%5F")) s = s.Replace("%5F", "_");
            if (s.Contains("%60")) s = s.Replace("%60", "`");
            if (s.Contains("%7B")) s = s.Replace("%7B", "{");
            if (s.Contains("%7C")) s = s.Replace("%7C", "|");
            if (s.Contains("%7D")) s = s.Replace("%7D", "}");
            if (s.Contains("%7E")) s = s.Replace("%7E", "~");

            return s;
        }

        /*private string PercentEncoding(string name)
        {
            string s = name;

            if (s.Contains(" ")) s = s.Replace(" ", "%20");
            if (s.Contains("\"")) s = s.Replace("\"", "%22");
            //if (s.Contains("%")) s = s.Replace("%", "%25");
            if (s.Contains("-")) s = s.Replace("-", "%2D");
            if (s.Contains(".")) s = s.Replace(".", "%2E");
            if (s.Contains("<")) s = s.Replace("<", "%3C");
            if (s.Contains(">")) s = s.Replace(">", "%3E");
            if (s.Contains("\\")) s = s.Replace("\\", "%5C");
            if (s.Contains("^")) s = s.Replace("^", "%5E");
            if (s.Contains("_")) s = s.Replace("_", "%5F");
            if (s.Contains("`")) s = s.Replace("`", "%60");
            if (s.Contains("{")) s = s.Replace("{", "%7B");
            if (s.Contains("|")) s = s.Replace("|", "%7C");
            if (s.Contains("}")) s = s.Replace("}", "%7D");
            if (s.Contains("~")) s = s.Replace("~", "%7E");

            return s;
        }*/
    }
}
