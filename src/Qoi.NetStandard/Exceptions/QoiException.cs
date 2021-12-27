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
    public class QoiException : Exception
    {
        public QoiException(string message) : base(message)
        {
        }

        public QoiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public QoiException()
        {
        }

        protected QoiException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
