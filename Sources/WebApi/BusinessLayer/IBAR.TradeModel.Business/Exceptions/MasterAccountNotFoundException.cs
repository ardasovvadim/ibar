using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class MasterAccountNotFoundException : EntityNotFoundException
    {
        public MasterAccountNotFoundException(string msg) : base(msg)
        {
        }

        public MasterAccountNotFoundException() : base("Master account not found")
        {
        }

        public MasterAccountNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MasterAccountNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}