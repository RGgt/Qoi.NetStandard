using System;
using System.Collections.Generic;
using System.Text;

namespace Qoi.NetStandard.Exceptions
{
    /// <summary>
    /// If implements the standard pattern for deriving exceptions, but provides
    /// no specific behavior or data.
    /// </summary>
    [Serializable]
    public class QoiEncodingException : Exception
    {
        public QoiEncodingException(string message) : base(message)
        {
        }

        public QoiEncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public QoiEncodingException()
        {
        }

        protected QoiEncodingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
