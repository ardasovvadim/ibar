using System.ServiceModel;

namespace IBAR.TradeModel.Business.Services.Wcf
{
    [ServiceContract(CallbackContract = typeof(IContractCallBack))]
    public interface IContract
    {
        [OperationContract]
        void ReloadJob(string job);

        [OperationContract]
        void StartJob(string job);

        [OperationContract]
        void StopJob(string job);

        [OperationContract]
        void StatusJob(string job);
    }
}