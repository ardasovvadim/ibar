using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(string msg) : base(msg)
        {
        }

        public UserNotFoundException() : base("User not found")
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}