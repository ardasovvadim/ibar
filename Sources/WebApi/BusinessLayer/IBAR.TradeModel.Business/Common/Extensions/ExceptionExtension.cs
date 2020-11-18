using System;
using System.Text;

namespace IBAR.TradeModel.Business.Common.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetErrorTrace(this Exception ex, StringBuilder trace = null, bool inner = false)
        {
            if (!inner)
            {
                trace = new StringBuilder();
            }

            trace.AppendLine(inner ? "InnerException" : "Exception");
            trace.AppendFormat("Message: {0} {1}", ex.Message, ex.Message.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine);
            trace.AppendFormat("StackTrace: {0} {1}", ex.StackTrace, ex.StackTrace.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine);

            if (ex.InnerException != null)
            {
                trace.AppendLine(ex.InnerException.GetErrorTrace(trace, true));
            }

            trace.AppendLine();
            return trace.ToString();
        }
    }
}
