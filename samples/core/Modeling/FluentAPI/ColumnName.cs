﻿using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.ColumnName
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region ColumnName
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.BlogId)
                .HasColumnName("blog_id");
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
