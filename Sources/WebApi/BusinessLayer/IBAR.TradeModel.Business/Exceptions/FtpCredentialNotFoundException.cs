using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class FtpCredentialNotFoundException : EntityNotFoundException
    {
        public FtpCredentialNotFoundException(string msg) : base(msg)
        {
        }

        public FtpCredentialNotFoundException() : base("Ftp credential not found")
        {
        }

        public FtpCredentialNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FtpCredentialNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}