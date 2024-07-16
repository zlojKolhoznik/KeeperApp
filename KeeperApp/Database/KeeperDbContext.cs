using KeeperApp.Records;
using KeeperApp.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Database
{
    public class KeeperDbContext : DbContext
    {
        public DbSet<LoginRecord> Logins { get; set; }

        public KeeperDbContext()
        {
            
        }

        public IEnumerable<Record> GetRecordsForUser(string username)
        {
            List<Record> records = Logins.Cast<Record>().ToList();
            return records.Where(r => r.OwnerUsernameHash == Sha256Hasher.GetSaltedHash(username, r.Created.ToString()));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-5GVDE96;Database=KeeperDb;Trusted_Connection=True;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginRecord>().HasEncryption();
            base.OnModelCreating(modelBuilder);
        }
    }
}
