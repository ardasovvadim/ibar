using System.ServiceModel;

namespace IBAR.TradeModel.Business.Services.Wcf
{
    public interface IContractCallBack
    {
        [OperationContract(IsOneWay = true)]
        void OnCallback(string input);
    }
}