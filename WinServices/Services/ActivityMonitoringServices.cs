using System;
using System.IO;
using System.ServiceProcess;

namespace WinServices
{
    public class ActivityMonitoringServices : BaseService
    {
        private const string Path = @"C:\Users\Admin\Documents\TestApplication.txt";

        public ActivityMonitoringServices()
        {
            CanHandleSessionChangeEvent = true;
            ServiceName = "kriptone Activity Monitoring";
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            string userInfo = WinServices.Utils.InteractiveUser.Account(changeDescription.SessionId);
            switch (changeDescription.Reason) {
                case SessionChangeReason.SessionLock:
                    Write(changeDescription.Reason.ToString() + " OnSessionChange " + changeDescription.SessionId.ToString() + " - " + Environment.UserName + "-" + userInfo);
                    break;
                case SessionChangeReason.SessionLogoff:
                    Write(changeDescription.Reason.ToString() + " OnSessionChange " + changeDescription.SessionId.ToString() + " - " + Environment.UserName + "-" + userInfo);
                    break;
                case SessionChangeReason.SessionLogon:
                    Write(changeDescription.Reason.ToString() + " OnSessionChange " + changeDescription.SessionId.ToString() + " - " + Environment.UserName + "-" + userInfo);
                    break;
                case SessionChangeReason.SessionUnlock:
                    Write(changeDescription.Reason.ToString() + " OnSessionChange " + changeDescription.SessionId.ToString() + " - " + Environment.UserName + "-" + userInfo);
                    break;
                default:
                    break;
            }

            base.OnSessionChange(changeDescription);
        }

        private void Write(string data)
        {
            if (!File.Exists(Path)) {
                using (var sw = File.CreateText(Path)) {
                    sw.WriteLine(data);
                }
            }
            else {
                using (var sw = File.AppendText(Path)) {
                    sw.WriteLine(data);
                }
            }
        }
    }
}
