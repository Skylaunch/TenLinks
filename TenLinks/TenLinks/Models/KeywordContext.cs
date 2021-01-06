using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TenLinks.Models
{
    public class KeywordContext : DbContext
    {
        public KeywordContext(DbContextOptions<KeywordContext> option) : base(option)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = "Server=DESKTOP-4Q97L55\\SQLEXPRESS;Database=TenLinksDatabase;Trusted_Connection=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<Keyword> Keyword { get; set; }
        public DbSet<Link> Link { get; set; }
    }
}
