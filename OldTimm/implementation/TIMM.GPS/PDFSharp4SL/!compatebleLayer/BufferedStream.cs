/*
silverPDF is sponsored by Aleyant Systems (http://www.aleyant.com)

silverPDF is based on PdfSharp (http://www.pdfsharp.net) and iTextSharp (http://itextsharp.sourceforge.net)

Developers: Ai_boy (aka Oleksii Okhrymenko)

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above information and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR SPONSORS
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.IO
{
    [ComVisible(true)]
    public sealed class BufferedStream : Stream
    {
        private bool disposed;
        private byte[] m_buffer;
        private int m_buffer_pos;
        private int m_buffer_read_ahead;
        private bool m_buffer_reading;
        private Stream m_stream;

        public BufferedStream(Stream stream)
            : this(stream, 0x1000)
        {
        }

        public BufferedStream(Stream stream, int bufferSize)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize", "<= 0");
            }
            if (!stream.CanRead && !stream.CanWrite)
            {
                throw new ObjectDisposedException(Locale.GetText("Cannot access a closed Stream."));
            }
            this.m_stream = stream;
            this.m_buffer = new byte[bufferSize];
        }

        private void CheckObjectDisposedException()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("BufferedStream", Locale.GetText("Stream is closed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.m_buffer != null)
                {
                    this.Flush();
                }
                this.m_stream.Close();
                this.m_buffer = null;
                this.disposed = true;
            }
        }

        public override void Flush()
        {
            this.CheckObjectDisposedException();
            if (this.m_buffer_reading)
            {
                if (this.CanSeek)
                {
                    this.m_stream.Position = this.Position;
                }
            }
            else if (this.m_buffer_pos > 0)
            {
                this.m_stream.Write(this.m_buffer, 0, this.m_buffer_pos);
            }
            this.m_buffer_read_ahead = 0;
            this.m_buffer_pos = 0;
        }

        public override int Read([In, Out] byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            this.CheckObjectDisposedException();
            if (!this.m_stream.CanRead)
            {
                throw new NotSupportedException(Locale.GetText("Cannot read from stream"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "< 0");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "< 0");
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException("array.Length - offset < count");
            }
            if (!this.m_buffer_reading)
            {
                this.Flush();
                this.m_buffer_reading = true;
            }
            if (count <= (this.m_buffer_read_ahead - this.m_buffer_pos))
            {
                Buffer.BlockCopy(this.m_buffer, this.m_buffer_pos, array, offset, count);
                this.m_buffer_pos += count;
                if (this.m_buffer_pos == this.m_buffer_read_ahead)
                {
                    this.m_buffer_pos = 0;
                    this.m_buffer_read_ahead = 0;
                }
                return count;
            }
            int num = this.m_buffer_read_ahead - this.m_buffer_pos;
            Buffer.BlockCopy(this.m_buffer, this.m_buffer_pos, array, offset, num);
            this.m_buffer_pos = 0;
            this.m_buffer_read_ahead = 0;
            offset += num;
            count -= num;
            if (count >= this.m_buffer.Length)
            {
                return (num + this.m_stream.Read(array, offset, count));
            }
            this.m_buffer_read_ahead = this.m_stream.Read(this.m_buffer, 0, this.m_buffer.Length);
            if (count < this.m_buffer_read_ahead)
            {
                Buffer.BlockCopy(this.m_buffer, 0, array, offset, count);
                this.m_buffer_pos = count;
                return (num + count);
            }
            Buffer.BlockCopy(this.m_buffer, 0, array, offset, this.m_buffer_read_ahead);
            num += this.m_buffer_read_ahead;
            this.m_buffer_read_ahead = 0;
            return num;
        }

        public override int ReadByte()
        {
            this.CheckObjectDisposedException();
            byte[] array = new byte[1];
            if (this.Read(array, 0, 1) == 1)
            {
                return array[0];
            }
            return -1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckObjectDisposedException();
            if (!this.CanSeek)
            {
                throw new NotSupportedException(Locale.GetText("Non seekable stream."));
            }
            this.Flush();
            return this.m_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.CheckObjectDisposedException();
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("value must be positive");
            }
            if (!this.m_stream.CanWrite && !this.m_stream.CanSeek)
            {
                throw new NotSupportedException("the stream cannot seek nor write.");
            }
            if ((this.m_stream == null) || (!this.m_stream.CanRead && !this.m_stream.CanWrite))
            {
                throw new IOException("the stream is not open");
            }
            this.m_stream.SetLength(value);
            if (this.Position > value)
            {
                this.Position = value;
            }
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            this.CheckObjectDisposedException();
            if (!this.m_stream.CanWrite)
            {
                throw new NotSupportedException(Locale.GetText("Cannot write to stream"));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "< 0");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "< 0");
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException("array.Length - offset < count");
            }
            if (this.m_buffer_reading)
            {
                this.Flush();
                this.m_buffer_reading = false;
            }
            if (this.m_buffer_pos >= (this.m_buffer.Length - count))
            {
                this.Flush();
                this.m_stream.Write(array, offset, count);
            }
            else
            {
                Buffer.BlockCopy(array, offset, this.m_buffer, this.m_buffer_pos, count);
                this.m_buffer_pos += count;
            }
        }

        public override void WriteByte(byte value)
        {
            this.CheckObjectDisposedException();
            byte[] array = new byte[] { value };
            this.Write(array, 0, 1);
        }

        public override bool CanRead
        {
            get
            {
                return this.m_stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.m_stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.m_stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                this.Flush();
                return this.m_stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckObjectDisposedException();
                return ((this.m_stream.Position - this.m_buffer_read_ahead) + this.m_buffer_pos);
            }
            set
            {
                if (((value < this.Position) && ((this.Position - value) <= this.m_buffer_pos)) && this.m_buffer_reading)
                {
                    this.m_buffer_pos -= (int)(this.Position - value);
                }
                else if (((value > this.Position) && ((value - this.Position) < (this.m_buffer_read_ahead - this.m_buffer_pos))) && this.m_buffer_reading)
                {
                    this.m_buffer_pos += (int)(value - this.Position);
                }
                else
                {
                    this.Flush();
                    this.m_stream.Position = value;
                }
            }
        }
    }

    internal sealed class Locale
    {
        private Locale()
        {
        }

        public static string GetText(string msg)
        {
            return msg;
        }

        public static string GetText(string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }
    }

 

}
