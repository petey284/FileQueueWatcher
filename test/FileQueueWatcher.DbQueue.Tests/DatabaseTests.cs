using System;
using System.IO;
using SQLite.Net;
using Xunit;

namespace FileQueueWatcher.Tests
{
    public class DatabaseTests
    {
        private readonly string DbFilepath;
        public DatabaseTests()
        {
            this.DbFilepath = Path.Combine(Environment.CurrentDirectory, "test.db");
        }

        [Fact]
        public void InitialConstructorTests()
        {
            var database = new Database(this.DbFilepath);

            Assert.True(database.InitialDirectories is Table<Directory>);
            Assert.True(database.WatchedDirectories is Table<WatchedDirectory>);
        }

        [Fact]
        public void InitialTests()
        {
            Assert.Throws<ArgumentException>(() => new Database(""));
        }
    }
}