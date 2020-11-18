using IBAR.Syncer.Infrastructure.Data;
using IBAR.TradeModel.Business.Common.Log;
using System;
using System.Threading;
using System.Threading.Tasks;
using IBAR.TradeModel.Business.Services;

namespace IBAR.Syncer.Infrastructure.Application.Jobs
{
    public abstract class BaseJob
    {
        protected Thread _workingThread;
        protected JobStatus _status = JobStatus.Running;
        private bool _tryReload = false;
        private int _countReloading = 0;

        private int _jobInterval;
        protected int JobInterval
        {
            get => _jobInterval * 60 * 1000;
            set => _jobInterval = value;
        }
        public IGlobalLogger GlobalLogger { get; set; }
        public ISyncerInfoService SyncerInfoService { get; set; }

        public async Task Run()
        {
            _status = JobStatus.Running;
                
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
                                Thread.Sleep(JobInterval);
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            GlobalLogger.LogError(ex.Message, ex, this.GetType().Name, true);
                            _status = JobStatus.Error;
                            _tryReload = false;
                            _countReloading = 0;
                            await Task.FromResult(false);
                        }
                        catch (Exception ex)
                        {
                            GlobalLogger.LogError(ex.Message, ex, this.GetType().Name, true);
                            _status = JobStatus.Error;
                            _tryReload = true;
                            Thread.Sleep(JobInterval);
                        }
                        finally
                        {
                            aer.Set();
                        }
                    })
                { IsBackground = true };

                _workingThread = taskThread;
                taskThread.Start();

                await Task.FromResult(aer.WaitOne());

                _status = JobStatus.Stopped;

                GlobalLogger.LogInfo("Job was stopped!", this.GetType().Name, true);

                if (_tryReload && _countReloading <= 10)
                {
                    StartThread();
                    ++_countReloading;

                    GlobalLogger.LogError("Can't start job. 10 attempts have been used.", null, this.GetType().Name, true);
                }
            }
        }

        protected abstract Task RunInternal();

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