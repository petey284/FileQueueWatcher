using System;
using SQLite.Net;
using SQLite.Net.Attributes;

namespace FileQueueWatcher
{
    public class Directory
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Fullpath { get; set; }
    }

    public class WatchedDirectory
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Fullpath { get; set; }
        public bool IsWatched { get; set; }
    }

    public class Database : SQLiteDatabase
    {
        public Table<Directory> InitialDirectories { get; set; }
        public Table<WatchedDirectory> WatchedDirectories { get; set; }
        public Database(string databasePath) : base(databasePath)
        {
            if (string.IsNullOrEmpty(databasePath)) { throw new ArgumentException(); }
        }
    }
}