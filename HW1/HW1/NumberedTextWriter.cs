using System;
using System.IO;
using System.Text;

namespace CS422
{
	public class NumberedTextWriter : TextWriter
	{
		private int _currentLineNumber;
		private TextWriter _tw;

		public NumberedTextWriter (TextWriter wrapThis)
		{
			_currentLineNumber = 1;
			_tw = wrapThis;
		}

		public NumberedTextWriter(TextWriter wrapThis, int startingLineNumber)
		{
			_currentLineNumber = startingLineNumber;
			_tw = wrapThis;
		}

		/// <summary>
		/// Returns the encoding of the wrapped TextWriter object
		/// </summary>
		public override Encoding Encoding { get { return _tw.Encoding; } }

		/// <summary>
		/// Writes	the	current	line number,
		/// then a colon, then a space and then the	actual value passed ss the parameter.
		/// Increments the line number after
		/// </summary>
		public override void WriteLine (string value)
		{
			_tw.WriteLine(_currentLineNumber + ": " + value);
			_currentLineNumber++;
		}
	}
}

