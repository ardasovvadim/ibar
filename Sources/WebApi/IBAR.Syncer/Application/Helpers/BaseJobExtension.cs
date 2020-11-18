using IBAR.Syncer.Application.Jobs;
using IBAR.TradeModel.Business.Common.Extensions;
using System;

namespace IBAR.Syncer.Application.Helpers
{
    public static class BaseJobExtension
    {
        public static string GetErrorLogMessage(this BaseJob job, Exception ex)
        {
            return job.GetErrorLogMessage(ex.GetErrorTrace());
        }

        public static string GetErrorLogMessage(this BaseJob job, string message)
        {
            return $"JOB: {job.GetType().Name} | {message}";
        }
    }
}
