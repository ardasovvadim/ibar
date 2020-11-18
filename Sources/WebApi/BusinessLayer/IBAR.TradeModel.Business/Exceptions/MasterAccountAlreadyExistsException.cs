using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class MasterAccountAlreadyExistsException : EntityAlreadyExistsException
    {
        public MasterAccountAlreadyExistsException() : base("Master account already exists")
        {
        }

        public MasterAccountAlreadyExistsException(string message) : base(message)
        {
        }

        public MasterAccountAlreadyExistsException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected MasterAccountAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}