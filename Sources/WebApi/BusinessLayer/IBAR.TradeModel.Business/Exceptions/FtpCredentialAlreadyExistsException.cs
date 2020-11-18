using System;
using System.Runtime.Serialization;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class FtpCredentialAlreadyExistsException : EntityAlreadyExistsException
    {
        public FtpCredentialAlreadyExistsException() : base("Ftp credential already exists")
        {
        }

        public FtpCredentialAlreadyExistsException(string message) : base(message)
        {
        }

        public FtpCredentialAlreadyExistsException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected FtpCredentialAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}