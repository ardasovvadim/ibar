namespace IBAR.TradeModel.Business.Services.Wcf
{
    public interface IWcfService
    {
        void ReloadJob(string job);
        void StopJob(string job);
        void StartJob(string job);
        void StatusJob(string job);
    }
    
    public class WcfService : IWcfService
    {
        private WcfClient wcfClient = new WcfClient();

        public void ReloadJob(string job)
        {
            wcfClient.Contract.ReloadJob(job);
        }

        public void StartJob(string job)
        {
            wcfClient.Contract.StartJob(job);
        }

        public void StatusJob(string job)
        {
            wcfClient.Contract.StatusJob(job);
        }

        public void StopJob(string job)
        {
            wcfClient.Contract.StopJob(job);
        }
    }

}