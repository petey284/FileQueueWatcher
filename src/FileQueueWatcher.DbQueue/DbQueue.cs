using System;

namespace FileQueueWatcher.DbQueue
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
    }
};