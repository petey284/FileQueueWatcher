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

        public List<string> GetDirectories()
        {
            throw new NotImplementedException();
        }

        public void AddWatchedDirectory(string monitoredPath)
        {
            throw new NotImplementedException();
        }
    }
};