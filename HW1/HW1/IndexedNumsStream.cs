using System;
using System.IO;

namespace CS422
{
	public class IndexedNumsStream : Stream
	{
		private long _currentStreamPosition;
		private long _streamLength;

		public IndexedNumsStream (long streamLength)
		{
			if (streamLength < 0) 
			{
				_streamLength = 0;
			} 
			else 
			{
				_streamLength = streamLength;
			}
		}

		#region implemented abstract members of Stream

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}
			
		/// <summary>
		/// Sets the stream length.
		/// </summary>
		public override void SetLength (long value)
		{
			if (value < 0) 
			{
				_streamLength = 0;
			} 
			else 
			{
				_streamLength = value;
			}
		}

		/// <summary>
		/// Read the specified buffer, offset and count.
		/// </summary>
		public override int Read (byte[] buffer, int offset, int count)
		{
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
                if (count > (Length - Position))
                {
                    count = unchecked((int)(Length - Position));
                }
				for (int i = 0; i < count; i++)
				{
					buffer[i + offset] = (byte)(Position++ % 256);
				}
			}
			return count;
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}

		public override bool CanRead {
			get {
				return true;
			}
		}

		public override bool CanSeek {
			get {
				return true;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override long Length {
			get {
				return _streamLength;
			}
		}

		/// <summary>
		/// Gets or sets the position.
		/// If the Position property is ever set to a negative value, "clamp" it to 0.
		/// If the Posiition property is ever set to a value greater than the length of the stream, clamp to stream length
		/// Otherwise, Position to value.
		/// </summary>
		public override long Position {
			get {
				return _currentStreamPosition;
			}
			set {
				if (value < 0) 
				{
					_currentStreamPosition = 0;
				} 
				else if (value > _streamLength)
				{
					_currentStreamPosition =  _streamLength;
				}
				else
				{
					_currentStreamPosition = value;
				}
			}
		}

		#endregion
	}
}

