using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CS422
{
    public abstract class Dir422
    {
        public abstract string Name { get; }

        public abstract IList<Dir422> GetDirs();

        public abstract IList<File422> GetFiles();

        public abstract Dir422 Parent { get; }

        public abstract bool ContainsFile(string fileName, bool recursive);

        public abstract bool ContainsDir(string dirName, bool recursive);

        public abstract Dir422 GetDir(string dirName);

        public abstract File422 GetFile(string fileName);

        public abstract File422 CreateFile(string fileName);

        public abstract Dir422 CreateDir(string dirName);
    }

    public abstract class File422
    {
        public abstract string Name { get; }

        public abstract Dir422 Parent { get; }

        public abstract Stream OpenReadOnly();

        public abstract Stream OpenReadWrite();

        public abstract string GetContentType();
    }

    public abstract class FileSys422
    {
        public abstract Dir422 GetRoot();

        public virtual bool Contains(File422 file)
        {
            return Contains(file.Parent);
        }

        public virtual bool Contains(Dir422 dir)
        {
            if (dir == null) { return false; }

            if (dir == GetRoot()) { return true; }

            return Contains(dir.Parent);
        }
    }

    public class StandardFileSystem : FileSys422
    {
        private StdFSDir m_root;

        private StandardFileSystem(string rootDir)
        {
            m_root = new StdFSDir(rootDir, rootDir);
        }

        public override Dir422 GetRoot()
        {
            return m_root;
        }

        public static StandardFileSystem Create(string rootDir)
        {
            if (!Directory.Exists(rootDir))
                return null;
            return new StandardFileSystem(rootDir);
        }
    }

    public class StdFSDir : Dir422
    {
        private string m_path;
        private string m_root;

        public override string Name { 
            get 
            { 
                //return m_path.Split(new char[] {'/', '\\'}).Last(); 
                return Path.GetFileName(m_path);
            } 
        }

        //public StdFSDir(string path)
        //{
        //    m_path = path;
        //}

        public StdFSDir(string path, string root)
        {
            m_path = path;
            m_root = root;
        }

        public override IList<File422> GetFiles()
        {
            List<File422> files = new List<File422>();

            foreach (string file in Directory.GetFiles(m_path))
            {
                files.Add(new StdFSFile(file, m_root));
            }
            return files;
        }

        public override IList<Dir422> GetDirs()
        {
            List<Dir422> dirs = new List<Dir422>();

            foreach (string dir in Directory.GetDirectories(m_path))
            {
                dirs.Add(new StdFSDir(dir, m_root));
            }

            return dirs;
        }

        #region implemented abstract members of Dir422

        public override bool ContainsFile(string fileName, bool recursive)
        {
            IList<File422> files;

            if (fileName.Contains("\\") || fileName.Contains("/"))
                return false;

            if (recursive) 
            {
                // First check this directory
                files = GetFiles();

                foreach (File422 file in files) 
                {
                    if (file.Name == fileName)
                        return true;
                }

                // Search deeper
                IList<Dir422> dirs = GetDirs();
                foreach (Dir422 dir in dirs) 
                {
                    if (dir.ContainsFile(fileName, recursive))
                        return true;
                }

                return false;
            }

            // else, don't do recursive
            files = GetFiles();

            foreach (File422 file in files) 
            {
                if (file.Name == fileName)
                    return true;
            }

            return false;
        }

        public override bool ContainsDir(string dirName, bool recursive)
        {
            IList<Dir422> dirs;

            if (dirName.Contains("\\") || dirName.Contains("/"))
                return false;

            if (recursive) {
                // First check this directory
                dirs = GetDirs ();

                foreach (Dir422 dir in dirs) {
                    if (dir.Name == dirName)
                        return true;
                }
                    
                foreach (Dir422 dir in dirs) {
                    if (dir.ContainsDir (dirName, true))
                        return true;
                }

                return false;
            }

            // Not recursive
            dirs = GetDirs ();

            foreach (Dir422 dir in dirs) {
                if (dir.Name == dirName)
                    return true;
            }

            return false;
        }

        public override Dir422 GetDir(string dirName)
        {
            if (dirName.Contains("\\") || dirName.Contains("/"))
                return null;

            if (ContainsDir(dirName, false))
            {
                return new StdFSDir(string.Format ("{0}/{1}", m_path, dirName), m_root);
            }
                
            return null;
        }

        public override File422 GetFile(string fileName)
        {
            if (fileName.Contains("\\") || fileName.Contains("/"))
                return null;

            if (ContainsFile(fileName, false))
            {
                return new StdFSFile(string.Format ("{0}/{1}", m_path, fileName), m_root);
            }

            return null;
        }

        public override File422 CreateFile(string fileName)
        {
            if (fileName.Contains("\\") || fileName.Contains("/") ||
                fileName == string.Empty || fileName == null)
                return null;
            else
            {
                File.Create(string.Format ("{0}/{1}", m_path, fileName)).Dispose();
                return new StdFSFile(string.Format ("{0}/{1}", m_path, fileName), m_root);
            }
        }

        public override Dir422 CreateDir(string dirName)
        {
            if (dirName.Contains("\\") || dirName.Contains("/") ||
                dirName == string.Empty || dirName == null)
                return null;
            else
            {
                if (ContainsDir(dirName, false))
                {
                    return GetDir(dirName);
                }
                Directory.CreateDirectory(string.Format ("{0}/{1}", m_path, dirName));
                return new StdFSDir(string.Format ("{0}/{1}", m_path, dirName), m_root);
            }
        }

        public override Dir422 Parent
        {
            get
            {
                if (m_path == m_root)
                    return null;

                return new StdFSDir(Directory.GetParent(m_path).FullName, m_root);
            }
        }

        #endregion

        public static bool operator == (StdFSDir dir1, StdFSDir dir2)
        {
            return dir1.Name == dir2.Name;
        }

        public static bool operator != (StdFSDir dir1, StdFSDir dir2)
        {
            return dir1.Name == dir2.Name;
        }
    }

    public class StdFSFile : File422
    {
        private string m_path;
        private string m_root;

        public StdFSFile(string path, string root)
        {
            m_path = path;
            m_root = root;
        }

        // fail to read/write return null if someone is already reading

        #region implemented abstract members of File422
       
        public override Stream OpenReadOnly()
        {
            try{
                return File.OpenRead(m_path);
            }
            catch{
                return null;
            }
        }
        public override Stream OpenReadWrite()
        {
            try {
                return File.Open(m_path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }
            catch {
                return null;
            }
        }
        public override string Name
        {
            get
            {
                //return m_path.Split(new char[] { '/', '\\' }).Last();
                return Path.GetFileName(m_path);
            }
        }
        public override Dir422 Parent
        {
            get
            {
                return new StdFSDir(Directory.GetParent(m_path).FullName, m_root);
            }
        }

        public override string GetContentType()
        {
            string extension = Path.GetExtension(Name);
            if (extension == string.Empty)
                return "text/html";
            extension = extension.Remove(0, 1).ToLower();
            if (extension == "pdf")
                return "application/" + extension;
            if (extension == "mov" || extension == "mp4" || extension == "avi")
                return "video/" + extension;
            if (extension == "jpeg" || extension == "jpg" || extension == "png")
                return "image/" + extension;
            if (extension == "mp3" || extension == "mpeg")
                return "audio/" + extension;
            if (extension == "txt" || extension == "rtf" || extension == "html" || extension == "htm" || extension == "xml")
                return "text/" + extension;
            return "text/html";
        }

        #endregion
    }

    public class MemoryFileSystem : FileSys422
    {
        private MemFSDir m_root;

        public MemoryFileSystem()
        {
            m_root = new MemFSDir("/", null);
        }

        #region implemented abstract members of FileSys422

        public override Dir422 GetRoot()
        {
            return m_root;
        }

        #endregion
    }

    public class MemFSDir : Dir422
    {
        private string m_name;
        private MemFSDir m_parent;

        private IList<MemFSDir> m_dirs;
        private IList<MemFSFile> m_files;


        public MemFSDir(string name, MemFSDir parent)
        {
            m_name = name;
            m_dirs = new List<MemFSDir>();
            m_files = new List<MemFSFile>();
            m_parent = parent;
        }

        #region implemented abstract members of Dir422

        public override IList<Dir422> GetDirs()
        {
            return m_dirs.ToList<Dir422>();;
        }

        public override IList<File422> GetFiles()
        {
            return m_files.ToList<File422>();
        }

        public override bool ContainsFile(string fileName, bool recursive)
        {
            if (fileName.Contains("\\") || fileName.Contains("/"))
                return false;
            
            if (GetFile(fileName) != null)
            {
                return true;
            }

            if (recursive)
            {
                foreach (var dir in m_dirs)
                {
                    if (dir.ContainsFile(fileName, true))
                        return true;
                }
            }
            return false;
        }

        public override bool ContainsDir(string dirName, bool recursive)
        {
            if (dirName.Contains("\\") || dirName.Contains("/"))
                return false;
            
            if (GetDir(dirName) != null)
            {
                return true;
            }

            if (recursive)
            {
                foreach (var dir in m_dirs)
                {
                    if (dir.ContainsDir(dirName, true))
                        return true;
                }
            }
            return false;
        }

        public override Dir422 GetDir(string dirName)
        {
            if (dirName.Contains("\\") || dirName.Contains("/"))
                return null;
            
            return m_dirs.FirstOrDefault(x => x.Name == dirName);
        }

        public override File422 GetFile(string fileName)
        {
            if (fileName.Contains("\\") || fileName.Contains("/"))
                return null;
            
            return m_files.FirstOrDefault(x => x.Name == fileName);
        }

        public override File422 CreateFile(string fileName)
        {
            if (fileName.Contains("\\") || fileName.Contains("/") ||
                fileName == string.Empty || fileName == null)
                return null;
            
            MemFSFile file = new MemFSFile(fileName, this);
            m_files.Add(file);
            return file;
        }

        public override Dir422 CreateDir(string dirName)
        {
            if (dirName.Contains("\\") || dirName.Contains("/") ||
                dirName == string.Empty || dirName == null)
                return null;
            
            MemFSDir dir = new MemFSDir(dirName, this);
            m_dirs.Add(dir);
            return dir;
        }

        public override string Name
        {
            get
            {
                return m_name;
            }
        }

        public override Dir422 Parent
        {
            get
            {
                return m_parent;
            }
        }

        #endregion
    }

    public class MemFSFile : File422
    {
        private string m_name;
        private MemFSDir m_parent;
        MemoryStream m_stream;
        List<MemoryStream> openReads;
        bool write;

        public MemFSFile(string name, MemFSDir parent)
        {
            m_name = name;
            m_parent = parent;
            m_stream = new MemoryStream();
            openReads = new List<MemoryStream>();
            write = false;
        }

        private void DisposeIt(object sender, EventArgs e)
        {
            m_stream = new MemoryStream((sender as MemoryStream).GetBuffer());
            m_stream.Seek(0, SeekOrigin.Begin);
            (sender as MemoryStream).Close();
            write = false;
        }

        #region implemented abstract members of File422

        public override Stream OpenReadOnly()
        {
            MemoryStream ms = new MemoryStream();

            lock (openReads)
            {
                if(write == true)
                {
                    return null;
                }

                m_stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                openReads.Add(ms);
            }

            return ms;
        }

        public override Stream OpenReadWrite()
        {
            lock (openReads)
            {
                if (write == true)
                {
                    return null;
                }
                if(openReads.Any(x => x.CanRead != false))
                {
                    return null;
                }
                    
                write = true;

                MemoryStream ms = new MemoryStream();
                m_stream.Seek(0, SeekOrigin.Begin);
                m_stream.CopyTo(ms);
                m_stream.Seek(0, SeekOrigin.Begin);
                NotifyDisposedMemStream ndms = new NotifyDisposedMemStream(ms);
                ndms.streamDisposed += DisposeIt;

                return ndms;
            }
        }

        public override string Name
        {
            get
            {
                return m_name;
            }
        }

        public override Dir422 Parent
        {
            get
            {
                return m_parent;
            }
        }

        public override string GetContentType()
        {
            int indexOFPeriod = Name.LastIndexOf('.');
            if (indexOFPeriod == -1)
                return "text/html";
            string extension = Name.Substring(indexOFPeriod + 1, Name.Length - indexOFPeriod - 1).ToLower();
            if (extension == "pdf")
                return "application/" + extension;
            if (extension == "mov" || extension == "mp4" || extension == "avi")
                return "video/" + extension;
            if (extension == "jpeg" || extension == "jpg" || extension == "png")
                return "image/" + extension;
            if (extension == "mp3" || extension == "mpeg")
                return "audio/" + extension;
            if (extension == "txt" || extension == "rtf" || extension == "html" || extension == "htm" || extension == "xml")
                return "text/" + extension;
            return "text/html";//"application/octet-stream";
        }

        #endregion
    } 

    public class NotifyDisposedMemStream : MemoryStream
    {
        private MemoryStream m_stream;
        public event EventHandler streamDisposed;

        public NotifyDisposedMemStream(MemoryStream stream)
        {
            m_stream = stream;
        }

        #region Forwarding Overrides

        public override bool CanRead {
            get {
                return m_stream.CanRead;
            }
        }

        public override bool CanSeek {
            get {
                return m_stream.CanSeek;
            }
        }

        public override bool CanWrite {
            get {
                return m_stream.CanWrite;
            }
        }

        public override int Capacity {
            get {
                return m_stream.Capacity;
            }
            set {
                m_stream.Capacity = value;
            }
        }

        public override bool CanTimeout {
            get {
                return m_stream.CanTimeout;
            }
        }

        public override long Length {
            get {
                return m_stream.Length;
            }
        }

        public override long Position {
            get {
                return m_stream.Position;
            }
            set {
                m_stream.Position = value;
            }
        }

        public override int WriteTimeout {
            get {
                return m_stream.WriteTimeout;
            }
            set {
                m_stream.WriteTimeout = value;
            }
        }

        public override void SetLength (long value)
        {
            m_stream.SetLength (value);
        }

        public override int Read (byte[] buffer, int offset, int count)
        {
            return m_stream.Read (buffer, offset, count);
        }

        public override void Write (byte[] buffer, int offset, int count)
        {
            m_stream.Write (buffer, offset, count);
        }

        public override long Seek (long offset, SeekOrigin loc)
        {
            return m_stream.Seek (offset, loc);
        }

        public override void WriteByte (byte value)
        {
            m_stream.WriteByte (value);
        }

        #endregion 

        protected override void Dispose(bool disposing)
        {
            EventArgs e = new EventArgs ();
            OnStreamDisposed (e);
        }

        protected virtual void OnStreamDisposed(EventArgs e)
        {
            EventHandler handler = streamDisposed;

            if (handler != null)
                handler(m_stream, e);
        }
    }
}
