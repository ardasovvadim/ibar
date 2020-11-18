using IBAR.Syncer.Wcf;
using NLog;
using NLog.Targets;

namespace IBAR.Syncer.Tools.Nlog
{
    [Target("WcfStatusTarget")]
    public class WcfStatusTarget : TargetWithLayout
    {
        //private readonly Service _wcf = new Service();

        //public WcfStatusTarget()
        //{
        //    this.Host = "localhost";
           
        //}

        //public string Host { get; set; }

        //protected override void Write(LogEventInfo loginfo)
        //{
        //    _wcf.Status(loginfo.Message);
        //}
 
    }
}

