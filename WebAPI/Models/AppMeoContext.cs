using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class AppMeoContext : DbContext
    {
        public AppMeoContext(DbContextOptions<AppMeoContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<ResourcePath> ResourcePath { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seed Data
            modelBuilder.Entity<Role>().HasData(
                new { RoleID = 1, Name = "Admin" },
                 new { RoleID = 2, Name = "Mod" },
                  new { RoleID = 3, Name = "Member" }
                );

            //Seed Data Path
            modelBuilder.Entity<ResourcePath>().HasData(
                new { ResourcePathID=1,RoleID=3, Path = "/home/create" },
                new { ResourcePathID=2, RoleID = 3, Path = "/home/create/" }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
