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

        public KeeperDbContext() : base()
        {
            Database.Migrate();
        }

        public IEnumerable<Record> GetRecordsForUser(string username)
        {
            List<Record> records = [.. Logins.Cast<Record>(),
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
            base.OnModelCreating(modelBuilder);
        }
    }
}
