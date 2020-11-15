using System;
using System.Collections.Generic;
using System.Linq;
using static FileQueueWatcher.Constants;

namespace FileQueueWatcher
{
    public class DbQueue : IDisposable
    {
        /// <summary>
        ///     Database file path
        /// </summary>
        public string ConnectionString;

        /// <summary>
        ///     Sqlite database
        /// </summary>
        public Database Db;

        /// <summary>
        ///     Factory method for DbQueues
        /// </summary>
        /// <param name="dbFilePath">Sqlite db file path</param>
        /// <returns>DbQueue</returns>
        public static DbQueue Init(string dbFilePath)
        {
            var dbQueue = new DbQueue();
            dbQueue.SetFilePath(dbFilePath);
            dbQueue.Db = new Database(dbFilePath);

            return dbQueue;
        }

        /// <summary>
        ///     Set file path for database
        /// </summary>
        /// <param name="dbFilePath">File path</param>
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
            var directoriesBeingWatchedMapping =
                this.Db.WatchedDirectories.ToDictionary(x => x.Fullpath, x => x.Id);

            return this.Db.InitialDirectories.ToList()
                .Where(x => !directoriesBeingWatchedMapping.ContainsKey(x.Fullpath))
                .Select(x => x.Fullpath)
                .ToList();
        }

        /// <summary>
        ///     Checks if there are any new directories
        /// </summary>
        /// <returns>True if there are any new directories</returns>
        public bool HasNewDirectoryEntries() => this.GetNewDirectories().Count() > 0;

        public void AddWatchedDirectory(string monitoredPath)
        {
            this.Db.WatchedDirectories.Insert(new WatchedDirectory
            {
                Fullpath = monitoredPath,
                IsWatched = true
            });
        }

        /// <summary>
        ///     Drops WatchedDirectories table
        /// </summary>
        public void DropWatchedDirectoriesTable() => this.Db.DropTable(WatchedDirectories);

        /// <summary>
        ///     Dispose methods
        /// </summary>
        public void Dispose()
        {
            // TODO: Consider if database should be removed.
            this.Db.Dispose();
        }
    }
};