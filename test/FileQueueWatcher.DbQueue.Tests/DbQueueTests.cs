using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace FileQueueWatcher.Tests
{
    public class DbQueueTests
    {
        private readonly string DbFilepath;
        public DbQueueTests()
        {
            this.DbFilepath = Path.Combine(Environment.CurrentDirectory, "test.db");
        }

        [Fact]
        public void InitConstructorTests()
        { 
            // Act
            var dbQueue = DbQueue.Init(this.DbFilepath);

            // Assert
            Assert.Equal(this.DbFilepath, dbQueue.ConnectionString);

            Assert.NotNull(dbQueue.Db);
            Assert.NotNull(dbQueue);
        }

        [Fact]
        public void InitConstructorEmptyPathThrowsErrorTests()
        {
            Assert.Throws<ArgumentException>(() => DbQueue.Init(string.Empty));
        }

        [Fact]
        public void GetNewDirectoriesTest()
        {
            // Arrange
            var dbQueue = DbQueue.Init(this.DbFilepath);
            dbQueue.Db.InitialDirectories.DeleteAll();
            dbQueue.Db.WatchedDirectories.DeleteAll();

            dbQueue.Db.InitialDirectories.Insert(
                new List<Directory>
                {
                    new Directory { Fullpath = "C:\\" },
                    new Directory { Fullpath = "D:\\" },
                });

            dbQueue.Db.WatchedDirectories.Insert(new WatchedDirectory { Fullpath = "C:\\", IsWatched = true });

            var expected = new List<string>() { "D:\\" };

            // Act
            var actualOutput = dbQueue.GetNewDirectories();

            // Assert
            Assert.Equal(expected, actualOutput);
        }

        [Fact]
        public void HasNewDirectoryEntriesTest()
        {
            // Arrange
            var dbQueue = DbQueue.Init(this.DbFilepath);
            dbQueue.Db.InitialDirectories.DeleteAll();
            dbQueue.Db.WatchedDirectories.DeleteAll();

            dbQueue.Db.InitialDirectories.Insert(
                new List<Directory>
                {
                    new Directory { Fullpath = "C:\\" },
                    new Directory { Fullpath = "D:\\" },
                });

            dbQueue.Db.WatchedDirectories.Insert(new WatchedDirectory { Fullpath = "C:\\", IsWatched = true });

            //  Act
            var hasNewDirectoryEntries = dbQueue.HasNewDirectoryEntries();

            // Assert
            Assert.True(hasNewDirectoryEntries);
        }

        [Fact]
        public void AddWatchedDirectoryTests()
        {
            // Arrange
            var dbQueue = DbQueue.Init(this.DbFilepath);
            dbQueue.Db.WatchedDirectories.DeleteAll();

            // Act
            dbQueue.AddWatchedDirectory("C:\\");

            var count = dbQueue.Db.WatchedDirectories.ToList().Count();

            //  Assert
            Assert.True(count > 0);
        }

        [Fact]
        public void DropWatchedDirectoriesTableTests()
        {
            // Arrange
            var dbQueue = DbQueue.Init(this.DbFilepath);

            dbQueue.Db.WatchedDirectories.Insert(
                new List<WatchedDirectory>
                {
                    new WatchedDirectory { Fullpath = "C:\\" },
                    new WatchedDirectory { Fullpath = "D:\\" },
                    new WatchedDirectory { Fullpath = "F:\\" },
                    new WatchedDirectory { Fullpath = "W:\\" },
                });

            dbQueue.DropWatchedDirectoriesTable();

            // Act
            var doesWatchedDirectoriesTableExist = DoesTableExist(dbQueue.Db, "WatchedDirectories");

            // Assert
            Assert.False(doesWatchedDirectoriesTableExist);
        }

        // Checks if database exists
        private bool DoesTableExist(Database db, string tableName)
        {
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE name == ?", tableName) == 1;
        }
    }
}