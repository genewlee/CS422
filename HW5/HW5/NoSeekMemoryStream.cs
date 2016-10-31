using System;
using System.IO;

namespace CS422
{
    /// <summary>
    /// Represents a memory stream that does not support seeking, but otherwise has
    /// functionality identical to the MemoryStream class.
    /// </summary>
    public class NoSeekMemoryStream : MemoryStream
    {
        MemoryStream _ms;

        public NoSeekMemoryStream(byte[] buffer)
        {
            _ms = new MemoryStream(buffer);
        }

        public NoSeekMemoryStream(byte[] buffer, int offset, int count)
        {
            _ms = new MemoryStream(buffer, offset, count);
        }

        // Override necessary properties and methods to ensure that this stream functions
        // just like the MemoryStream class, but throws a NotSupportedException when seeking
        // is attempted (you'll have to override more than just the Seek function!)

        public override bool CanSeek { get { return false; } }
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }

        public override long Length
        {
            get
            {
                if (CanSeek)
                    return _ms.Length;
                throw new NotSupportedException("Does not support Length");
            }
        }

        public override void SetLength(long value)
        {
            if (!CanSeek)
                throw new NotSupportedException("Does not support SetLength");
            _ms.SetLength(value);
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException("Does not support seeking");
        }

        public override long Position
        {
            get
            {
                return _ms.Position;
            }
            set
            {
                if (CanSeek)
                    _ms.Position = value;
                throw new NotSupportedException("Does not support Position");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ms.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _ms.Write(buffer, offset, count);
        }
    }
}

