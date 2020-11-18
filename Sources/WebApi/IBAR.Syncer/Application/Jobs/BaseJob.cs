using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Data;
using IBAR.Syncer.Wcf;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IBAR.Syncer.Application.Jobs
{
    public abstract class BaseJob
    {
        protected readonly object _syncRoot = new object();
        protected Thread _workingThread;
        protected JobStatus _status = JobStatus.Running;
        protected readonly IContract _wcf;
        protected Job _job;
        protected static Logger logger = LogManager.GetLogger("Logger");
        protected static Logger loggerException = LogManager.GetLogger("LoggerException");

        // for deleting
        private bool _tryReload = false;
        private int _countReloading = 0;

        protected BaseJob(IContract wcf)
        {
            _wcf = wcf;
        }

        public async Task Run()
        {
            var jobName = Enum.GetName(typeof(Job), _job);

            _status = JobStatus.Running;
            _wcf.Status($"{jobName}$Job was started!");
            _wcf.StatusJob(jobName);

            using (var aer = new AutoResetEvent(false))
            {
                var taskThread = new Thread(async () =>
                    {
                        try
                        {
                            while (true)
                            {

                                await RunInternal();

                                _countReloading = 0;
                                Thread.Sleep(SleepInterval());
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            //todo: log here
                            Console.WriteLine(ex);
                            _status = JobStatus.Error;
                            loggerException.Error(this.GetErrorLogMessage(ex));
                            _wcf.StatusJob(jobName);
                            await Task.FromResult(false);
                            _tryReload = false;
                            _countReloading = 0;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            _status = JobStatus.Error;
                            loggerException.Error(this.GetErrorLogMessage(ex));
                            _wcf.StatusJob(jobName);
                            _tryReload = true;
                            Thread.Sleep(SleepInterval());
                        }
                        finally
                        {
                            aer.Set();
                        }
                    })
                { IsBackground = true };

                _workingThread = taskThread;
                taskThread.Start();

                Thread.Sleep(2000);

                await Task.FromResult(aer.WaitOne());

                Console.ForegroundColor = ConsoleColor.Red;
                _status = JobStatus.Stopped;
                Console.WriteLine($"{GetType().Name} Job was stopped!");
                logger.Log(LogLevel.Info, $"{jobName}${GetType().Name} Job was stopped!");
                Console.ForegroundColor = ConsoleColor.White;
                _wcf.StatusJob(jobName);

                if (_tryReload && _countReloading <= 10)
                {
                    StartThread();
                    ++_countReloading;
                    loggerException.Error(this.GetErrorLogMessage("Can't start job. 3 attempts have been used."));
                }
            }
        }

        protected abstract Task RunInternal();

        protected virtual int SleepInterval()
        {
            return 1000 * 5;
        }

        public void ReloadThread()
        {
            if (_workingThread.IsAlive)
            {
                _workingThread.Abort();
                Task.Run(Run);
            }
        }

        public void StartThread()
        {
            if (!_workingThread.IsAlive)
            {
                Task.Run(Run);
            }
        }

        public void StopThread()
        {
            if (_workingThread.IsAlive)
            {
                _workingThread.Abort();
            }
        }

        public string GetStatus()
        {
            return Enum.GetName(typeof(JobStatus), _status);
        }
    }
}