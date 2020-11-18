using System;
using System.Collections.Generic;
using System.Linq;

namespace IBAR.TradeModel.Business.Exceptions
{
    public class MultipleException : Exception
    {
        private List<Exception> exceptions;

        public MultipleException()
        {
            exceptions = new List<Exception>();
        }

        public void Add(Exception e)
        {
            exceptions.Add(e);
        }

        public void ThrowIf()
        {
            if (exceptions.Count > 0)
            {
                throw this;
            }
        }

        public IEnumerable<string> GetErrorMessages()
        {
            return exceptions.Select(e => e.Message);
        }


        public override string ToString()
        {
            return exceptions.Select(e => e.Message).Aggregate((p, n) => p + "\n" + n);
        }
    }
}