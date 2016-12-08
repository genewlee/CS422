using System;
using System.IO;
using System.Reflection;
using System.Net.Sockets;
using System.Text;

namespace CS422
{
    public class ConcatStream : Stream
    {
        Stream m_first, m_second;
        long len, position;
        bool isFixedLengthConstructor;

        public override bool CanRead { get { return m_first.CanRead && m_second.CanRead; } }
        public override bool CanSeek { get { return m_first.CanSeek && m_second.CanSeek; } }
        public override bool CanWrite { get { return m_first.CanWrite && m_second.CanWrite; } }

        public ConcatStream(Stream first, Stream second)
        {
            if (!first.CanSeek)
                throw new NotSupportedException("First stream must support Length property");
            /*
             * if a class derived from Stream does not support seeking, 
             * calls to Length, SetLength, Position, and Seek throw a NotSupportedException.
             */

            m_first = first;
            m_second = second;
            position = 0;
            isFixedLengthConstructor = false;
            try
            {
                len = first.Length + second.Length;
            }
            catch
            {
                len = -1;
            }
        }

        public ConcatStream(Stream first, Stream second, long fixedLength)
        {
            if (fixedLength < 0)
                throw new ArgumentException("Length cannot be less than 0");

            m_first = first;
            m_second = second;
            position = 0;
            len = fixedLength;
            isFixedLengthConstructor = true;
        }

        #region implemented abstract members of Stream

        public override void Flush()
        {
            m_first.Flush(); m_second.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException("Forward-only reading functionality with no seeking");

            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
            return position;
        }

        public override void SetLength(long value)
        {
            if (!CanSeek)
                throw new NotSupportedException("Only forward reading is supported");

            len = value < 0 ? 0 : value; // if value < 0 set to 0
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            if ((offset + count) > buffer.Length) // The sum of offset and count is larger than the buffer length. 
            {
                throw new ArgumentException();
            }
            else if (buffer == null) // buffer is null. 
            {
                throw new ArgumentNullException();
            }
            else if (offset < 0 || count < 0) // offset or count is negative. 
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (CanRead == false) // The stream does not support reading.  
            {
                throw new NotSupportedException();
            }
            else
            {
                try
                {
                    if (position + count <= m_first.Length) // from first stream
                        bytesRead = m_first.Read(buffer, offset, count);

                    else if (position > m_first.Length) // all from second stream
                        bytesRead = m_second.Read(buffer, offset, count);

                    else // both streams
                    {
                        int room = (int)(m_first.Length - m_first.Position);
                        bytesRead = room > 0 ? m_first.Read(buffer, offset, room) : 0;
                        if (len > m_first.Length && bytesRead < buffer.Length)
                            bytesRead += m_second.Read(buffer, offset + bytesRead, count - bytesRead);
                    }
                }
                catch (Exception e) {}
            }
            position += bytesRead;
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                return;

            if (Position + count <= m_first.Length) // to first stream
                m_first.Write(buffer, offset, count);

            else if (Position > m_first.Length) // all to second stream
                m_second.Write(buffer, offset, count);

            else // both streams
            {
                int room = (int)(m_first.Length - m_first.Position);
                m_first.Write(buffer, offset, room);
                m_second.Write(buffer, offset + room, count - room);
            }
            Position += count;
        }

        public override long Length
        {
            get
            {
                if (isFixedLengthConstructor)
                    return len;
                else
                {
                    if (len == -1)
                        return m_first.Length;
                    else
                        return m_first.Length + m_second.Length;
                }
            }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                if (value < 0)
                {
                    position = 0;
                    m_first.Seek(0, SeekOrigin.Begin);
                    if (m_second.CanSeek)
                        m_second.Seek(0, SeekOrigin.Begin);
                }
                else if (Length < value)
                {
                    position = Length;
                    m_first.Seek(0, SeekOrigin.End);
                    if (m_second.CanSeek)
                        m_second.Seek(0, SeekOrigin.End);
                }
                else
                {
                    position = value;

                    if (value <= m_first.Length)
                    {
                        m_first.Seek(value, SeekOrigin.Begin);
                        if (m_second.CanSeek)
                            m_second.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        m_first.Seek(0, SeekOrigin.End);
                        if (m_second.CanSeek)
                            m_second.Seek(value - m_first.Length, SeekOrigin.Begin);
                    }
                }
            }
        }
        #endregion
    }
}

