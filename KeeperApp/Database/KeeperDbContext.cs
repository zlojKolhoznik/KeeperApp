using KeeperApp.Records;
using KeeperApp.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeeperApp.Database
{
    public class KeeperDbContext : DbContext
    {
        private static string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeeperApp", "KeeperDb.db");

        public DbSet<LoginRecord> Logins { get; set; }
        public DbSet<CardCredentialsRecord> CardCredentials { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<UserInfo> Users { get; set; }

        public KeeperDbContext() : base()
        {
            // We use Migrate() instead of EnsureCreated() to apply migrations. 
            // Also we don't want to use dotnet-ef CLI to apply migrations because it requires for user to do the same and uses different path for the database file.
            Database.Migrate();
        }

        public IEnumerable<Record> GetRecordsForUser(string username)
        {
            int count = Logins.Count() + CardCredentials.Count() + Folders.Count();
            List<Record> records = [..GetRecordsFromCollectionForUser(Logins, username),
                ..GetRecordsFromCollectionForUser(CardCredentials, username),
                ..GetRecordsFromCollectionForUser(Folders, username)];
            return records;
        }

        private IEnumerable<TRecord> GetRecordsFromCollectionForUser<TRecord>(DbSet<TRecord> collection, string username) where TRecord : Record
        {
            List<TRecord> result = new();
            for (int i = 0; i < collection.Count(); i++)
            {
                try
                {
                    var record = collection.ElementAt(i);
                    if (record.OwnerUsernameHash == Sha256Hasher.GetSaltedHash(username, record.Created.ToString()))
                    {
                        result.Add(record);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginRecord>().HasEncryption();
            modelBuilder.Entity<CardCredentialsRecord>().HasEncryption();
            modelBuilder.Entity<Folder>().HasEncryption();
            modelBuilder.Entity<UserInfo>().HasEncryption();
            base.OnModelCreating(modelBuilder);
        }
    }
}
