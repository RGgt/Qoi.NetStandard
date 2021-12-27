using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO
{
    /// <summary>
    /// Normally the code could have used a simply BinaryReader directly, but there is
    /// no comfortable way to check for the end of data.
    /// The simplest solution seems to be to proxy the binary reader and check for the
    /// ending sequence each time new bytes are read.
    /// </summary>
    internal class QoiBytesReader : IDisposable
    {
        private BinaryReader _binaryReader;
        private int _endOfStremSequenceProgress = 0;
        private bool _encounteredEndOfStremSequence = false;
        public bool EndOfDataReached{ get => _encounteredEndOfStremSequence; }
        /// <summary>
        /// Code here will need to get much more complicated if we need to allow jumps to
        /// positions in stream but so far we don't so a basic check is enough.
        /// </summary>
        /// <param name="b"></param>
        private void CheckEndOfStream(byte b)
        {
            if (b == StreamEndEncoder.STREAM_END_MARKER_BYTES[_endOfStremSequenceProgress])
            {
                _endOfStremSequenceProgress++;
                if (_endOfStremSequenceProgress == StreamEndEncoder.STREAM_END_MARKER_SIZE)
                {
                    _encounteredEndOfStremSequence = true;
                }
            }
            else
            {
                _endOfStremSequenceProgress = 0;
            }
        }

        public virtual byte ReadByte()
        {
            byte b = _binaryReader.ReadByte();
            CheckEndOfStream(b);
            return b;
        }
        public virtual byte[] ReadBytes(int count)
        {
            byte[] bytes = _binaryReader.ReadBytes(count);
            foreach (byte b in bytes)
            {
                CheckEndOfStream(b);
            }
            return bytes;
        }
        public virtual Stream BaseStream
        {
            get
            {
                return _binaryReader.BaseStream;
            }
        }

        #region StreamReader-like constructors
        public QoiBytesReader(Stream input) : this(input, new UTF8Encoding(), false)
        {
        }
        public QoiBytesReader(Stream input, Encoding encoding) : this(input, encoding, false)
        {
        }
        public QoiBytesReader(Stream input, Encoding encoding, bool leaveOpen)
        {
            _binaryReader = new BinaryReader(input, encoding, leaveOpen);
        }
        #endregion

        #region IDisposable pattern
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _binaryReader.Close();
            }
            _binaryReader = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~QoiBytesReader()
        {
            Dispose(false);
        }
        #endregion
    }
}
