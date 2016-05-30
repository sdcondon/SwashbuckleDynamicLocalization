namespace Swashbuckle.DynamicLocalization
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Readable (only) stream that wraps an underlying stream containing character data, and resolves
    /// placeholders (indicated by a percent sign immediately followed by a key in brackets) as it is
    /// being read.
    /// </summary>
    public class PlaceholderResolverStream : Stream
    {
        private const int MaxBufferedChars = 100;
        private const char PlaceholderStart = '%';

        private Func<string, string> _resolvePlaceholder;

        private StreamReader _reader;
        private long _position;

        private StringBuilder _currentChunk;
        private StringBuilder _currentPlaceholder;
        private char[] _currentChunkBuffer;
        private byte[] _outBuffer;
        private int _outBufferPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderResolverStream"/> class.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="resolvePlaceholder"></param>
        public PlaceholderResolverStream(Stream stream, Func<string, string> resolvePlaceholder)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }

            _reader = new StreamReader(stream, true);
            _resolvePlaceholder = resolvePlaceholder;

            _position = 0;

            // Kick off the output buffer with the BOM. Peek first to ensure that we recognise
            // the BOM - Encoding remains as default until stream is read for the first time.
            _reader.Peek();
            _outBuffer = _reader.CurrentEncoding.GetPreamble();
            _outBufferPos = 0;

            _currentChunk = new StringBuilder();
            _currentChunkBuffer = new char[0];
            _currentPlaceholder = new StringBuilder();
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public override void Flush()
        {
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentException("offset is invalid");
            }

            if (count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentException("count is invalid");
            }

            int bytesRead = 0;
            bool readerExhausted = false;
            while (bytesRead < count && !readerExhausted)
            {
                int remainingBuffer = _outBuffer.Length - _outBufferPos;
                if (remainingBuffer > 0)
                {
                    // There are bytes in the output buffer - copy them out.
                    int bytesToCopy = Math.Min(remainingBuffer, count - bytesRead);
                    Array.Copy(_outBuffer, _outBufferPos, buffer, offset, bytesToCopy);
                    _outBufferPos += bytesToCopy;
                    offset += bytesToCopy;
                    bytesRead += bytesToCopy;
                    _position += bytesToCopy;
                }
                else
                {
                    if (TryReadChunk())
                    {
                        WriteCurrentChunkToOutBuffer();
                    }
                    else
                    {
                        readerExhausted = true;
                    }
                }
            }

            return bytesRead;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private bool TryReadChunk()
        {
            if (_reader.EndOfStream)
            {
                return false;
            }
            else
            {
                _currentChunk.Clear();

                for (int i = 0, c = -1; i < MaxBufferedChars && (c = _reader.Read()) != -1;)
                {
                    if ((char)c == PlaceholderStart)
                    {
                        TryReadPlaceholder();
                        _currentChunk.Append(_currentPlaceholder.ToString());
                        i += _currentPlaceholder.Length;
                    }
                    else
                    {
                        _currentChunk.Append((char)c);
                        i++;
                    }
                }

                return true;
            }
        }

        private void TryReadPlaceholder()
        {
            bool validPlaceholderKey = false;

            _currentPlaceholder.Clear();
            _currentPlaceholder.Append(PlaceholderStart);

            for (int c = -1; (c = _reader.Read()) != -1;)
            {
                _currentPlaceholder.Append((char)c);

                if (c == '\r' || c == '\n')
                {
                    break;
                }
                else if (_currentPlaceholder.Length == 2 && c != '(')
                {
                    break;
                }
                else if (c == ')')
                {
                    validPlaceholderKey = true;
                    break;
                }
            }

            if (validPlaceholderKey)
            {
                string value = _resolvePlaceholder(_currentPlaceholder.ToString());
                _currentPlaceholder.Clear();
                _currentPlaceholder.Append(value);
            }
        }

        private void WriteCurrentChunkToOutBuffer()
        {
            // Can't encode directly from StringBuilders. Use a persistent char array instead of ToString to save GC load.
            int charCount = _currentChunk.Length;
            if (charCount > _currentChunkBuffer.Length)
            {
                _currentChunkBuffer = new char[charCount];
            }

            _currentChunk.CopyTo(0, _currentChunkBuffer, 0, _currentChunk.Length);

            // Encode the result using the StreamReader's encoding.
            // Re-use the existing buffer if we can for performance.
            int byteCount = _reader.CurrentEncoding.GetByteCount(_currentChunkBuffer, 0, charCount);
            if (byteCount > _outBuffer.Length)
            {
                _outBuffer = new byte[byteCount];
            }

            _outBufferPos = _outBuffer.Length - byteCount;

            _reader.CurrentEncoding.GetBytes(_currentChunkBuffer, 0, charCount, _outBuffer, _outBufferPos);
        }
    }
}
