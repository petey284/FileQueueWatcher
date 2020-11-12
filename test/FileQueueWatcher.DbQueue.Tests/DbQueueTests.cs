using System;
using SQLite.Net;
using Xunit;

namespace FileQueueWatcher.Tests
{
    public class DbQueueTests
    {
        [Fact]
        public void InitConstructorTests()
        {
            var dbFilepath = "C:\\temp\\test.db";
            var dbQueue = DbQueue.Init(dbFilepath);

            Assert.Equal(dbFilepath, dbQueue.ConnectionString);

            Assert.NotNull(dbQueue.Database);
            Assert.NotNull(dbQueue);
        }

        [Fact]
        public void InitConstructorEmptyPathThrowsErrorTests()
        {
            Assert.Throws<ArgumentException>(() => DbQueue.Init(string.Empty));
        }

        [Fact]
        public void GetNewDirectoriesTests()
        {
        }
    }

    public class DatabaseTests
    {

        [Fact]
        public void InitialConstructorTests()
        {
            var database = new Database("C:\\temp\\test.db");

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