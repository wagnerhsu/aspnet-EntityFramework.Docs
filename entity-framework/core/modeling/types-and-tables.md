---
title: Types and Tables - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: cbe6935e-2679-4b77-8914-a8d772240cf1
uid: core/modeling/types-and-tables
---
# Types and Tables

Including a type on your context means that it is included in EF's model; we usually refer to such a type as an *entity*. EF can read and write entity instances from/to the database, and if you're using a relational database, EF can create tables for your entities via migrations.

## Including types in the model

By convention, types that are exposed in DbSet properties on your context are included in your model as entities. Types that are specified in the `OnModelCreating` method are also included, as are any types that are found by recursively exploring the navigation properties of other discovered types.

In the code sample below, all types are included:

* `Blog` is included because it's exposed in a DbSet property on the context.
* `Post` is included because it's discovered via the `Blog.Posts` navigation property.
* `AuditEntry` because it is specified in `OnModelCreating`.

<!-- [!code-csharp[Main](samples/core/Modeling/Conventions/Samples/IncludedTypes.cs?highlight=3,7,16)] -->
``` csharp
class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEntry>();
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; set; }
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Blog Blog { get; set; }
}

public class AuditEntry
{
    public int AuditEntryId { get; set; }
    public string Username { get; set; }
    public string Action { get; set; }
}
```

## Excluding types from the model

If you don't want a type to be included in the model, you can exclude it:

# [Data Annotations](#tab/data-annotations)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Samples/IgnoreType.cs?highlight=20)] -->
``` csharp
[NotMapped]
public class BlogMetadata
{
    public DateTime LoadedFromDatabase { get; set; }
}
```

# [Fluent API](#tab/fluent-api)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/IgnoreType.cs?highlight=12)] -->
``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Ignore<BlogMetadata>();
}
```

***

> [!NOTE]
> The following sections are applicable only to relational databases such as SQL Server or Sqlite.

## Table name

By convention, each entity type will be set up to map to a database table with the same name as the DbSet property that exposes the entity. If no DbSet exists for the given entity, the class name is used.

You can manually configure the table name:

# [Data Annotations](#tab/data-annotations)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Samples/Relational/Table.cs?highlight=11)] -->
``` csharp
using System.ComponentModel.DataAnnotations.Schema;

[Table("blogs")]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
```

# [Fluent API](#tab/fluent-api)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/Relational/Table.cs?highlight=11-12)] -->
``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ToTable("blogs");
}
```

***

## Table schema

When using a relational database, tables are by default created in your database's default schema. For example, Microsoft SQL Server will use the `dbo` schema (SQLite does not support schemas).

You can configure tables to be created in a specific schema as follows:

# [Data Annotations](#tab/data-annotations)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Samples/Relational/TableAndSchema.cs?highlight=11-12)] -->
``` csharp
using System.ComponentModel.DataAnnotations.Schema;

[Table("blogs", Schema = "blogging")]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
```

# [Fluent API](#tab/fluent-api)

<!-- [!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/Relational/TableAndSchema.cs?highlight=11-12)] -->
``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ToTable("blogs", schema: "blogging");
}
```

***

Rather than specifying the schema for each table, you can also define the default schema at the model level with the fluent API:

<!-- [!code-csharp[Main](samples/core/relational/Modeling/FluentAPI/Samples/Relational/DefaultSchema.cs?highlight=7)] -->
``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.HasDefaultSchema("blogging");
}
```

Note that setting the default schema will also affect other database objects, such as sequences.
