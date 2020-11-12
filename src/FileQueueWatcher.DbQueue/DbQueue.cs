using System;
using System.Collections.Generic;

namespace FileQueueWatcher
{
    public class DbQueue
    {
        public string ConnectionString;
        public Database Database;

        /// <summary>
        ///     Factory method for DbQueues
        /// </summary>
        /// <param name="dbFilePath">Sqlite db file path</param>
        /// <returns>DbQueue</returns>
        public static DbQueue Init(string dbFilePath)
        {
            var dbQueue = new DbQueue();
            dbQueue.SetFilePath(dbFilePath);
            dbQueue.Database = new Database(dbFilePath);

            return dbQueue;
        }

        private void SetFilePath(string dbFilePath)
        {
            if (string.IsNullOrEmpty(dbFilePath))
            {
                throw new ArgumentException();
            }

            this.ConnectionString = dbFilePath;
        }

        /// <summary>
        ///     Gets any new directories to monitor
        /// </summary>
        /// <returns>List of directories as string</returns>
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

        public void DropWatchedDirectoriesTable()
        {
            throw new NotImplementedException();
        }
    }
};