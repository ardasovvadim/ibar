using System.ServiceModel;

namespace IBAR.SyncerManager
{
    public interface IMyContractCallback
    {
        [OperationContract]
        void OnCallback();
    }
}
