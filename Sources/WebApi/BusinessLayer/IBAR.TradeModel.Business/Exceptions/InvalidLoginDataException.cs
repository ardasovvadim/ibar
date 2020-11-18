using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class InvalidLoginDataException : Exception
    {
        public InvalidLoginDataException() : base("Invalid login data")
        {
        }

        public InvalidLoginDataException(string message) : base(message)
        {
        }

        public InvalidLoginDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLoginDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}