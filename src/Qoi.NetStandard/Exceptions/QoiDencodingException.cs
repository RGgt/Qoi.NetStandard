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
    public class QoiDencodingException : QoiException
    {
        public QoiDencodingException(string message) : base(message)
        {
        }

        public QoiDencodingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public QoiDencodingException()
        {
        }

        protected QoiDencodingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
