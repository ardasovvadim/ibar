using System;
using System.ServiceModel;
using IBAR.Syncer.Application.Jobs;
using IBAR.Syncer.Application.Jobs.Data;
using IBAR.Syncer.Application.Jobs.Fs;
using IBAR.Syncer.Data;

namespace IBAR.Syncer.Wcf
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    class Service : IContract
    {
        private static FtpJob ftpJob;
        private static FsCopyFromFtpJob fsCopyFromFtpJob;
        private static ImportJob importJob;
        private static IContractCallBack _callback = null;

        public void Status(string input)
        {
            Console.WriteLine(input);
            try
            {
                if (_callback == null) return;
                IContractCallBack callbackInstance = _callback;
                callbackInstance?.OnCallback(input);
            }
            catch (CommunicationObjectAbortedException e)
            {
                _callback = null;
                Console.WriteLine("Client disconnected");
            }
        }

        public void ReloadJob(string job)
        {
            var jobInterface = GetJob(job);
            jobInterface?.ReloadThread();
        }

        public void StartJob(string job)
        {
            var jobInterface = GetJob(job);
            jobInterface?.StartThread();
        }

        public void StopJob(string job)
        {
            var jobInterface = GetJob(job);
            jobInterface?.StopThread();
        }

        private BaseJob GetJob(string job)
        {
            if (Enum.TryParse<Job>(job, true, out var j))
            {
                switch (j)
                {
                    case Job.Ftp:
                        return ftpJob;
                    case Job.Copy:
                        return fsCopyFromFtpJob;
                    case Job.Import:
                        return importJob;
                }
            }

            return null;
        }

        public void StatusJob(string job)
        {
            if (_callback == null)
            {
                _callback = OperationContext.Current?.GetCallbackChannel<IContractCallBack>();
            }

            var jobInterface = GetJob(job);

            if (jobInterface != null)
            {
                Status($"{job}$status$" + jobInterface?.GetStatus());
            }
            else
            {
                Status($"{job}$status$" + JobStatus.Initializing);
            }
        }

        public void GetInstance(FtpJob ftp, FsCopyFromFtpJob fsCopy, ImportJob impJob)
            
        {
            ftpJob = ftp;
            fsCopyFromFtpJob = fsCopy;
            importJob = impJob;
        }
    }
}