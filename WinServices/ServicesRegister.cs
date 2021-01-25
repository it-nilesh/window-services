using System.ServiceProcess;

namespace WinServices
{
    public class ServicesRegister : WinHosServicetLietime
    {
        public override ServiceBase[] RunServices()
        {
            return new ServiceBase[]
            {
                new ActivityMonitoringServices()
            };
        }
    }
}
