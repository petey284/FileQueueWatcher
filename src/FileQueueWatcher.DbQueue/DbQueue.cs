using System;
using System.Collections.Generic;

namespace FileQueueWatcher
{
    public class DbQueue
    {
        public string ConnectionString;
        public static DbQueue Init(string connectionString)
        {
            var dbQueue = new DbQueue();
            dbQueue.SetConfig(connectionString);

            return dbQueue;
        }

        private void SetConfig(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        ///     Gets any new directories to monitor
        /// </summary>
        /// <returns></returns>
        public List<string> GetNewDirectories()
        {
            throw new NotImplementedException();
        }

        public void AddWatchedDirectory(string monitoredPath)
        {
            throw new NotImplementedException();
        }

        public bool HasNewDirectoryEntries()
        {
            throw new NotImplementedException();
        }

        public void DropWatchedDirectoryTable()
        {
            throw new NotImplementedException();
        }
    }
};