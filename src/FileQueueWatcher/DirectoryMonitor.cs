using Serilog;
using System;
using System.IO;

namespace FileQueueWatcher
{

    class DirectoryMonitor
    {
        public string MonitoredPath;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private DateTime LastChanged;

        public DirectoryMonitor(string path)
        {
            MonitoredPath = path;
            Initialise();
        }

        private void Initialise()
        {
            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = MonitoredPath;
            fileSystemWatcher.Changed += HandleChangedEvent;
            fileSystemWatcher.Created += HandleCreatedEvent;
            fileSystemWatcher.Renamed += HandleRenamedEvent;
            fileSystemWatcher.Deleted += HandleDeletedEvent;
        }

        public void StartMonitor()
        {
            Log.Information("Monitor for directory {MonitorPath} is STARTING.", MonitoredPath);
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopMonitor()
        {
            Log.Information("Monitor for directory {MonitorPath} is STOPPING.", MonitoredPath);
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void HandleChangedEvent(object sender, FileSystemEventArgs e)
        {
            this.LastChanged = DateTime.Now;

            if (this.LastChanged == null)
            {
                LogEvent(e.Name, "changed");
                return;
            }

            if ((DateTime.Now - this.LastChanged) > new TimeSpan(100))
            {
                LogEvent(e.Name, "changed");
            }
        }

        private void HandleCreatedEvent(object sender, FileSystemEventArgs e)
        {
            LogEvent(e.Name, "created");
        }

        private void HandleRenamedEvent(object sender, FileSystemEventArgs e)
        {
            LogEvent(e.Name, "renamed");
        }

        private void HandleDeletedEvent(object sender, FileSystemEventArgs e)
        {
            LogEvent(e.Name, "deleted");
        }

        private void LogEvent(string fileName, string action)
        {
            Log.Information("File {FileName} {Action} in {MonitorPath}.", fileName, action, MonitoredPath);
        }
    }
}
