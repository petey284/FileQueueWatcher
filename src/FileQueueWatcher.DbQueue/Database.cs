using System;
using SQLite.Net;
using SQLite.Net.Attributes;
using static FileQueueWatcher.Constants;

namespace FileQueueWatcher
{
    [Table(Directories)]
    public class Directory
    {
        [PrimaryKey(AutoIncrement = true)]
        public int Id { get; set; }
        public string Fullpath { get; set; }
    }

    [Table(WatchedDirectories)]
    public class WatchedDirectory
    {
        [PrimaryKey(AutoIncrement = true)]
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

            if (!TableExists(Directories))
            {
                this.CreateTable(Directories, c => new
                {
                    Id = c.Column<int>(primaryKey: true, autoIncrement: true),
                    Fullpath = c.Column<string>(nullable: false)
                });
            }

            if (TableExists(Constants.WatchedDirectories))
            {
                this.DropTable(this.WatchedDirectories.Name);
            }

            this.CreateTable(Constants.WatchedDirectories, c => new
            {
                Id = c.Column<int>(primaryKey: true, autoIncrement: true),
                Fullpath = c.Column<string>(nullable: false),
                IsWatched = c.Column<bool>(nullable: false)
            });
        }

        private bool TableExists(string tableName)
        {
            // TODO: Why doesn't this work?
            // var tables = this.Query("SELECT * FROM sqlite_master", r => new
            // {
            //     Type = r.Get<string>("type"),
            //     Name = r.Get<string>("name"),
            //     TableName = r.Get<string>("tbl_name"),
            //     RootPage = r.Get<string>("rootpage"),
            //     Sql = r.Get<string>("sql")
            // }).Where(x => x.Name == tableName);

            return this.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE name == ?", tableName) == 1;
        }
    }
}