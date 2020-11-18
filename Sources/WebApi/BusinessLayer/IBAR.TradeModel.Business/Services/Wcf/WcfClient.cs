using System.ServiceModel;

namespace IBAR.TradeModel.Business.Services.Wcf
{
    public class WcfClient
    {
        public IContract Contract { get; set; }

        public WcfClient()
        {
            IContractCallBack callback = new MyCallBack();
            var channelFactoryRemote = new DuplexChannelFactory<IContract>(callback, "tcp");
            IContract chanel = channelFactoryRemote.CreateChannel();
            this.Contract = chanel;
        }
    }
}