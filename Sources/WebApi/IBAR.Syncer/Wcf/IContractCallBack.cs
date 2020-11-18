using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.Syncer.Wcf
{
    public interface IContractCallBack
    {
        [OperationContract(IsOneWay = true)]
        void OnCallback(string input);
    }
}
