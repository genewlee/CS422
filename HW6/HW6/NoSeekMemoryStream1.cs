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
        /*private byte[] _buf;
        private int _offset;
        private long _position;
        private long _length;*/
        MemoryStream _ms;

        public NoSeekMemoryStream(byte[] buffer)
        {
            /*_buf = buffer;
            _position = 0;
            _length = buffer.Length;*/
            _ms = new MemoryStream(buffer);
        }

        public NoSeekMemoryStream(byte[] buffer, int offset, int count)
        {
            /*_buf = buffer;
            _offset = offset;
            _position = offset;
            _length = count;*/
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
                    return _ms.Length;//return _length; 
                throw new NotSupportedException("Does not support Length");
            }
        }

        public override void SetLength(long value)
        {
            if (!CanSeek)
                throw new NotSupportedException("Does not support SetLength");
            _ms.SetLength(value);//_length = value;
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            /*if (CanSeek)
            {
                switch (loc)
                {
                    case SeekOrigin.Begin:
                        {
                            Position = (int)offset;
                            break;
                        }  
                    case SeekOrigin.Current:
                        {
                            Position = unchecked(Position + (int)offset);
                            break;
                        }    
                    case SeekOrigin.End:
                        {
                            Position = unchecked(_length + (int)offset);
                            break;
                        }
                }
                return Position;
            }*/
            throw new NotSupportedException("Does not support seeking");
        }

        public override long Position
        {
            get
            {
                //if (CanSeek)
                    return _ms.Position;//return _position;
                //throw new NotSupportedException("Does not support Position");
            }
            set
            {
                if (CanSeek)
                    _ms.Position = value;//_position = value;
                throw new NotSupportedException("Does not support Position");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            /*int byteCount = 0;
            while (byteCount < count)
            {
                buffer[offset + byteCount] = _buf[_position + byteCount];
                byteCount++;
            }
            return byteCount;*/
            return _ms.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            /*for (int i = 0; i < count; i++)
            {
                _buf[_position + i] = buffer[offset + i];
            }*/
            _ms.Write(buffer, offset, count);
        }
    }
}

