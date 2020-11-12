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
            // 1. Initializes file system database
            // 2. Reads from DirectoriesToWatch table
            // 3. Once directory monitor has started, adds database to table of directories watched
            // 4. Once service has ended, drops WatchedDirectories table
            var dbQueue = DbQueue.Init(this.DbPath);

            var dbQueueMonitors = dbQueue
                .GetDirectories()
                .Select(x => new DirectoryMonitor(x))
                .ToList();

            foreach (var monitor in dbQueueMonitors)
            {
                monitor.StartMonitor();
                dbQueue.AddWatchedDirectory(monitor.MonitoredPath);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
            foreach (var monitor in dbQueueMonitors)
            {
                monitor.StopMonitor();
            }
        }

        public override void Dispose()
        {
            // Drops WatchedDirectories table
            base.Dispose();
        }
    }
}
