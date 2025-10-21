using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Q3_CodeLink_EMS.Models;
using static Q3_CodeLink_EMS.Models.AdminUser;

// NuGet packages installed:
// Microsoft.EntityFrameworkCore
// Microsoft.EntityFrameworkCore.InMemory
// And all transitive packages included

/*
 * This InMemory database is set up in such a way and the the rest of the code base is written in such a way
 * that connecting to and swithcing to an actual database that uses SQL is as easy as using the current DbContext
 * and configuring the appsettings to connect to the database
 */

namespace Q3_CodeLink_EMS.Data
{
    public class CodeLinkEmsDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } // Employee table
        public DbSet<AdminUser> AdminUsers { get; set; } // AdminUser table

        public CodeLinkEmsDbContext(DbContextOptions<CodeLinkEmsDbContext> options) : base(options)
        {
            
        }

        // Storing enum Role as a string in InMemory database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>().Property(a => a.Role).HasConversion(new EnumToStringConverter<UserRole>());
        }
    }
}
