using System.ServiceModel;

namespace IBAR.Syncer.Wcf
{
    [ServiceContract(CallbackContract = typeof(IContractCallBack))]
    public interface IContract
    {
        [OperationContract()]
        void Status(string input);

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