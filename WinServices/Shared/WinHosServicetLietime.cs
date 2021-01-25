using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WinServices
{
    public class WinHosServicetLietime : IHostLifetime
    {
        public IHostApplicationLifetime ApplicationLifetime { get; private set; }
        public ServiceBase[] ServiceBases { get; private set; }

        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

        public void Inject(IHostApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            ServiceBases = RunServices();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopServices();

            //Services Stop
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            ApplicationLifetime.ApplicationStopping.Register(StopServices);

            new Thread(Run).Start();
            //Logs
            return _delayStart.Task;
        }

        private void Run()
        {
            try {
                ServiceBase.Run(ServiceBases);
                _delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
            }
            catch (Exception ex) {
                _delayStart.TrySetException(ex);
            }
        }

        public virtual ServiceBase[] RunServices()
        {
            throw new NotImplementedException();
        }

        private void StopServices()
        {
            foreach (var service in ServiceBases) {
                if (service.CanStop) {
                    service.Stop();
                }
            }
        }

        public sealed override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public sealed override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public sealed override string ToString()
        {
            return base.ToString();
        }
    }
}
