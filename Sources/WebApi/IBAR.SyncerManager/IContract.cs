using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace IBAR.SyncerManager
{
   [ServiceContract(CallbackContract = typeof(IMyContractCallback))]
    public interface IContract
    {
        [OperationContract]
        void Status(string input);

        [OperationContract(IsOneWay = true)]
        void ReloadJob();
    }
}
