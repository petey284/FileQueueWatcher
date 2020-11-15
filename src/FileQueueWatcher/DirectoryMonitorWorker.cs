using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FileQueueWatcher
{
    public class DirectoryMonitorWorker : BackgroundService
    {
        private DbQueue DbQueue { get; set; }

        private readonly IConfiguration Configuration;

        public DirectoryMonitorWorker(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.DbQueue = DbQueue.Init(this.GetDbPath());

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
                
                // Pause execution for a second
                await Task.Delay(1000, stoppingToken);
            }

            this.StopMonitors();
        }

        private string GetDbPath()
        {
            return this.Configuration.GetValue<string>("dbPath")
                ?? Path.Combine(Environment.CurrentDirectory, "dbMonitors.db");
        }

        private void StartWatchOnNewDirectories()
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
        }

        private void StopMonitors()
        {
            if (this.DbQueue.Db.WatchedDirectories.Any())
            {
                var directoryMonitors = this.DbQueue.Db.WatchedDirectories
                    .Select(x => new DirectoryMonitor(x.Fullpath))
                    .ToList();
                directoryMonitors.ForEach(x => x.StopMonitor());
            }
        }
    }
}