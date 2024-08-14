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
            List<Record> records = [.. Logins.Cast<Record>(),
                .. Folders.Cast<Record>(),
                .. CardCredentials.Cast<Record>()];
            return records.Where(r => r.OwnerUsernameHash == Sha256Hasher.GetSaltedHash(username, r.Created.ToString()));
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
