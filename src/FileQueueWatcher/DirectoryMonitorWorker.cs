using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FileQueueWatcher
{
    public class DirectoryMonitorWorker : BackgroundService
    {
        public string DbPath;

        public DirectoryMonitorWorker(IConfiguration configuration)
        {
            this.DbPath = configuration.GetValue<string>("dbPath");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initializing file system database (currently only works with sqlite)
            var dbQueue = DbQueue.Init(this.DbPath);

            var dbQueueMonitors = dbQueue
                .GetDirectories()
                .Select(x => new DirectoryMonitor(x))
                .ToList();

            List<DirectoryMonitor> monitors = new List<DirectoryMonitor>();

            monitors.Add(new DirectoryMonitor("C:\\temp"));
            monitors.Add(new DirectoryMonitor("C:\\temp\\test"));
            foreach (DirectoryMonitor monitor in monitors)
            {
                monitor.StartMonitor();
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
            foreach (DirectoryMonitor monitor in monitors)
            {
                monitor.StopMonitor();
            }
        }

        public override void Dispose()
        {
            // Remove particular table from database
            base.Dispose();
        }
    }
}
