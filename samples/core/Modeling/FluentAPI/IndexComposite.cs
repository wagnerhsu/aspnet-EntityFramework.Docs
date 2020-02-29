﻿using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.IndexComposite
{
    class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        #region Composite
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasIndex(p => new { p.FirstName, p.LastName });
        }
        #endregion
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
