---
title: Relationships - EF Core
description: How to configure relationships between entity types when using Entity Framework Core
author: AndriySvyryd
ms.date: 11/21/2019
uid: core/modeling/relationships
---
# Relationships

A relationship defines how two entities relate to each other. In a relational database, this is represented by a foreign key constraint.

> [!NOTE]  
> Most of the samples in this article use a one-to-many relationship to demonstrate concepts. For examples of one-to-one and many-to-many relationships see the [Other Relationship Patterns](#other-relationship-patterns) section at the end of the article.

## Definition of terms

There are a number of terms used to describe relationships

* **Dependent entity:** This is the entity that contains the foreign key properties. Sometimes referred to as the 'child' of the relationship.

* **Principal entity:** This is the entity that contains the primary/alternate key properties. Sometimes referred to as the 'parent' of the relationship.

* **Principal key:** The properties that uniquely identify the principal entity. This may be the primary key or an alternate key.

* **Foreign key:** The properties in the dependent entity that are used to store the principal key values for the related entity.

* **Navigation property:** A property defined on the principal and/or dependent entity that references the related entity.

  * **Collection navigation property:** A navigation property that contains references to many related entities.

  * **Reference navigation property:** A navigation property that holds a reference to a single related entity.

  * **Inverse navigation property:** When discussing a particular navigation property, this term refers to the navigation property on the other end of the relationship.
  
* **Self-referencing relationship:** A relationship in which the dependent and the principal entity types are the same.

The following code shows a one-to-many relationship between `Blog` and `Post`

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Relationships/Full.cs#Full)]

* `Post` is the dependent entity

* `Blog` is the principal entity

* `Blog.BlogId` is the principal key (in this case it is a primary key rather than an alternate key)

* `Post.BlogId` is the foreign key

* `Post.Blog` is a reference navigation property

* `Blog.Posts` is a collection navigation property

* `Post.Blog` is the inverse navigation property of `Blog.Posts` (and vice versa)

## Conventions

By default, a relationship will be created when there is a navigation property discovered on a type. A property is considered a navigation property if the type it points to can not be mapped as a scalar type by the current database provider.

> [!NOTE]  
> Relationships that are discovered by convention will always target the primary key of the principal entity. To target an alternate key, additional configuration must be performed using the Fluent API.

### Fully defined relationships

The most common pattern for relationships is to have navigation properties defined on both ends of the relationship and a foreign key property defined in the dependent entity class.

* If a pair of navigation properties is found between two types, then they will be configured as inverse navigation properties of the same relationship.

* If the dependent entity contains a property with a name matching one of these patterns then it will be configured as the foreign key:
  * `<navigation property name><principal key property name>`
  * `<navigation property name>Id`
  * `<principal entity name><principal key property name>`
  * `<principal entity name>Id`

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Relationships/Full.cs?name=Full&highlight=6,15-16)]

In this example the highlighted properties will be used to configure the relationship.

> [!NOTE]
> If the property is the primary key or is of a type not compatible with the principal key then it won't be configured as the foreign key.

> [!NOTE]
> Before EF Core 3.0 the property named exactly the same as the principal key property [was also matched as the foreign key](https://github.com/aspnet/EntityFrameworkCore/issues/13274)

### No foreign key property

While it is recommended to have a foreign key property defined in the dependent entity class, it is not required. If no foreign key property is found, a [shadow foreign key property](shadow-properties.md) will be introduced with the name `<navigation property name><principal key property name>` or `<principal entity name><principal key property name>` if no navigation is present on the dependent type.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Relationships/NoForeignKey.cs?name=NoForeignKey&highlight=6,15)]

In this example the shadow foreign key is `BlogId` because prepending the navigation name would be redundant.

> [!NOTE]
> If a property with the same name already exists then the shadow property name will be suffixed with a number.

### Single navigation property

Including just one navigation property (no inverse navigation, and no foreign key property) is enough to have a relationship defined by convention. You can also have a single navigation property and a foreign key property.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Relationships/OneNavigation.cs?name=OneNavigation&highlight=6)]

### Limitations

When there are multiple navigation properties defined between two types (that is, more than just one pair of navigations that point to each other) the relationships represented by the navigation properties are ambiguous. You will need to manually configure them to resolve the ambiguity.

### Cascade delete

By convention, cascade delete will be set to *Cascade* for required relationships and *ClientSetNull* for optional relationships. *Cascade* means dependent entities are also deleted. *ClientSetNull* means that dependent entities that are not loaded into memory will remain unchanged and must be manually deleted, or updated to point to a valid principal entity. For entities that are loaded into memory, EF Core will attempt to set the foreign key properties to null.

See the [Required and Optional Relationships](#required-and-optional-relationships) section for the difference between required and optional relationships.

See [Cascade Delete](../saving/cascade-delete.md) for more details about the different delete behaviors and the defaults used by convention.

## Manual configuration

### [Fluent API](#tab/fluent-api)

To configure a relationship in the Fluent API, you start by identifying the navigation properties that make up the relationship. `HasOne` or `HasMany` identifies the navigation property on the entity type you are beginning the configuration on. You then chain a call to `WithOne` or `WithMany` to identify the inverse navigation. `HasOne`/`WithOne` are used for reference navigation properties and `HasMany`/`WithMany` are used for collection navigation properties.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/NoForeignKey.cs?name=NoForeignKey&highlight=8-10)]

### [Data annotations](#tab/data-annotations)

You can use the Data Annotations to configure how navigation properties on the dependent and principal entities pair up. This is typically done when there is more than one pair of navigation properties between two entity types.

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Relationships/InverseProperty.cs?name=InverseProperty&highlight=20,23)]

> [!NOTE]
> You can only use [Required] on properties on the dependent entity to impact the requiredness of the relationship. [Required] on the navigation from the principal entity is usually ignored, but it may cause the entity to become the dependent one.

> [!NOTE]
> The data annotations `[ForeignKey]` and `[InverseProperty]` are available in the `System.ComponentModel.DataAnnotations.Schema` namespace. `[Required]` is available in the `System.ComponentModel.DataAnnotations` namespace.

---

### Single navigation property

If you only have one navigation property then there are parameterless overloads of `WithOne` and `WithMany`. This indicates that there is conceptually a reference or collection on the other end of the relationship, but there is no navigation property included in the entity class.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/OneNavigation.cs?name=OneNavigation&highlight=8-10)]

### Foreign key

#### [Fluent API (simple key)](#tab/fluent-api-simple-key)

You can use the Fluent API to configure which property should be used as the foreign key property for a given relationship:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/ForeignKey.cs?name=ForeignKey&highlight=11)]

#### [Fluent API (composite key)](#tab/fluent-api-composite-key)

You can use the Fluent API to configure which properties should be used as the composite foreign key properties for a given relationship:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/CompositeForeignKey.cs?name=CompositeForeignKey&highlight=13)]

#### [Data annotations (simple key)](#tab/data-annotations-simple-key)

You can use the Data Annotations to configure which property should be used as the foreign key property for a given relationship. This is typically done when the foreign key property is not discovered by convention:

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Relationships/ForeignKey.cs?name=ForeignKey&highlight=17)]

> [!TIP]  
> The `[ForeignKey]` annotation can be placed on either navigation property in the relationship. It does not need to go on the navigation property in the dependent entity class.

> [!NOTE]
> The property specified using `[ForeignKey]` on a navigation property doesn't need to exist the the dependent type. In this case the specified name will be used to create a shadow foreign key.

---

#### Shadow foreign key

You can use the string overload of `HasForeignKey(...)` to configure a shadow property as a foreign key (see [Shadow Properties](shadow-properties.md) for more information). We recommend explicitly adding the shadow property to the model before using it as a foreign key (as shown below).

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/ShadowForeignKey.cs?name=ShadowForeignKey&highlight=10,16)]

#### Foreign key constraint name

By convention, when targeting a relational database, foreign key constraints are named FK_<dependent type name>_<principal type name>_<foreign key property name>. For composite foreign keys <foreign key property name> becomes an underscore separated list of foreign key property names.

You can also configure the constraint name as follows:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/ConstraintName.cs?name=ConstraintName&highlight=6-7)]

### Without navigation property

You don't necessarily need to provide a navigation property. You can simply provide a foreign key on one side of the relationship.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/NoNavigation.cs?name=NoNavigation&highlight=8-11)]

### Principal key

If you want the foreign key to reference a property other than the primary key, you can use the Fluent API to configure the principal key property for the relationship. The property that you configure as the principal key will automatically be setup as an [alternate key](alternate-keys.md).

#### [Simple key](#tab/simple-key)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/PrincipalKey.cs?name=PrincipalKey&highlight=11)]

#### [Composite key](#tab/composite-key)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/CompositePrincipalKey.cs?name=CompositePrincipalKey&highlight=11)]

> [!WARNING]  
> The order in which you specify principal key properties must match the order in which they are specified for the foreign key.

---

### Required and optional relationships

You can use the Fluent API to configure whether the relationship is required or optional. Ultimately this controls whether the foreign key property is required or optional. This is most useful when you are using a shadow state foreign key. If you have a foreign key property in your entity class then the requiredness of the relationship is determined based on whether the foreign key property is required or optional (see [Required and Optional properties](required-optional.md) for more information).

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/Required.cs?name=Required&highlight=6)]

> [!NOTE]
> Calling `IsRequired(false)` also makes the foreign key property optional unless it's configured otherwise.

### Cascade delete

You can use the Fluent API to configure the cascade delete behavior for a given relationship explicitly.

See [Cascade Delete](../saving/cascade-delete.md) for a detailed discussion of each option.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/CascadeDelete.cs?name=CascadeDelete&highlight=6)]

## Other relationship patterns

### One-to-one

One to one relationships have a reference navigation property on both sides. They follow the same conventions as one-to-many relationships, but a unique index is introduced on the foreign key property to ensure only one dependent is related to each principal.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Relationships/OneToOne.cs?name=OneToOne&highlight=6,15-16)]

> [!NOTE]  
> EF will choose one of the entities to be the dependent based on its ability to detect a foreign key property. If the wrong entity is chosen as the dependent, you can use the Fluent API to correct this.

When configuring the relationship with the Fluent API, you use the `HasOne` and `WithOne` methods.

When configuring the foreign key you need to specify the dependent entity type - notice the generic parameter provided to `HasForeignKey` in the listing below. In a one-to-many relationship it is clear that the entity with the reference navigation is the dependent and the one with the collection is the principal. But this is not so in a one-to-one relationship - hence the need to explicitly define it.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/OneToOne.cs?name=OneToOne&highlight=11)]

### Many-to-many

Many-to-many relationships without an entity class to represent the join table are not yet supported. However, you can represent a many-to-many relationship by including an entity class for the join table and mapping two separate one-to-many relationships.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Relationships/ManyToMany.cs?name=ManyToMany&highlight=11-14,16-19,39-46)]
