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
        private readonly DbQueue DbQueue;

        public DirectoryMonitorWorker(IConfiguration configuration)
        {
            this.DbQueue = DbQueue.Init(configuration.GetValue<string>("dbPath"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Retrieve directories to watch from database queue and exit
            // early if there are no directories to watch
            var dbQueueMonitors = this.StartWatchOnNewDirectories();
            if (dbQueueMonitors.Count() == 0) { return; }

            // Stop process when cancellation is requested
            // When new directories are added to database queue, then begin
            // monitor for those new directories
            while (!stoppingToken.IsCancellationRequested)
            {
                if (this.DbQueue.HasNewDirectoryEntries())
                {
                    this.StartWatchOnNewDirectories();
                    continue;
                }

                await Task.Delay(1000, stoppingToken);
            }

            this.StopMonitors(dbQueueMonitors);
        }

        private List<DirectoryMonitor> StartWatchOnNewDirectories()
        {
            var dbQueueMonitors = this.DbQueue
                .GetNewDirectories() // Should return empty list when appropriate
                .Select(x => new DirectoryMonitor(x))
                .ToList();

            foreach (var monitor in dbQueueMonitors)
            {
                monitor.StartMonitor();
                this.DbQueue.AddWatchedDirectory(monitor.MonitoredPath);
            }

            return dbQueueMonitors;
        }

        private void StopMonitors(List<DirectoryMonitor> dbQueueMonitors) =>
            dbQueueMonitors.ForEach(x => x.StopMonitor());

        public override void Dispose()
        {
            this.DbQueue.DropWatchedDirectoriesTable();
            base.Dispose();
        }
    }
}
