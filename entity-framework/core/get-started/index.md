---
title: Getting Started - EF Core
author: rick-anderson
ms.date: 09/17/2019
ms.assetid: 3c88427c-20c6-42ec-a736-22d3eccd5071
uid: core/get-started/index
---

# Getting Started with EF Core

In this tutorial, you create a .NET Core console app that performs data access against a SQLite database using Entity Framework Core.

You can follow the tutorial by using Visual Studio on Windows, or by using the .NET Core CLI on Windows, macOS, or Linux.

[View this article's sample on GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/GetStarted).

## Prerequisites

Install the following software:

### [.NET Core CLI](#tab/netcore-cli)

* [.NET Core 3.0 SDK](https://www.microsoft.com/net/download/core).

### [Visual Studio](#tab/visual-studio)

* [Visual Studio 2019 version 16.3 or later](https://www.visualstudio.com/downloads/) with this  workload:
  * **.NET Core cross-platform development** (under **Other Toolsets**)

---

## Create a new project

### [.NET Core CLI](#tab/netcore-cli)

```dotnetcli
dotnet new console -o EFGetStarted
cd EFGetStarted
```

### [Visual Studio](#tab/visual-studio)

* Open Visual Studio
* Click **Create a new project**
* Select **Console App (.NET Core)** with the **C#** tag and click **Next**
* Enter **EFGetStarted** for the name and click **Create**

---

## Install Entity Framework Core

To install EF Core, you install the package for the EF Core database provider(s) you want to target. This tutorial uses SQLite because it runs on all platforms that .NET Core supports. For a list of available providers, see [Database Providers](../providers/index.md).

### [.NET Core CLI](#tab/netcore-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### [Visual Studio](#tab/visual-studio)

* **Tools > NuGet Package Manager > Package Manager Console**
* Run the following commands:

  ``` PowerShell
  Install-Package Microsoft.EntityFrameworkCore.Sqlite
  ```

Tip: You can also install packages by right-clicking on the project and selecting **Manage NuGet Packages**

---

## Create the model

Define a context class and entity classes that make up the model.

### [.NET Core CLI](#tab/netcore-cli)

* In the project directory, create **Model.cs** with the following code

### [Visual Studio](#tab/visual-studio)

* Right-click on the project and select **Add > Class**
* Enter **Model.cs** as the name and click **Add**
* Replace the contents of the file with the following code

---

[!code-csharp[Main](../../../samples/core/GetStarted/Model.cs)]

EF Core can also [reverse engineer](../managing-schemas/scaffolding.md) a model from an existing database.

Tip: In a real app, you put each class in a separate file and put the [connection string](../miscellaneous/connection-strings.md) in a configuration file or environment variable. To keep the tutorial simple, everything is contained in one file.

## Create the database

The following steps use [migrations](xref:core/managing-schemas/migrations/index) to create a database.

### [.NET Core CLI](#tab/netcore-cli)

* Run the following commands:

  ```dotnetcli
  dotnet tool install --global dotnet-ef
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

  This installs [dotnet ef](../miscellaneous/cli/dotnet.md) and the design package which is required to run the command on a project. The `migrations` command scaffolds a migration to create the initial set of tables for the model. The `database update` command creates the database and applies the new migration to it.

### [Visual Studio](#tab/visual-studio)

* Run the following commands in **Package Manager Console**

  ``` PowerShell
  Install-Package Microsoft.EntityFrameworkCore.Tools
  Add-Migration InitialCreate
  Update-Database
  ```

  This installs the [PMC tools for EF Core](../miscellaneous/cli/powershell.md). The `Add-Migration` command scaffolds a migration to create the initial set of tables for the model. The `Update-Database` command creates the database and applies the new migration to it.

---

## Create, read, update & delete

* Open *Program.cs* and replace the contents with the following code:

  [!code-csharp[Main](../../../samples/core/GetStarted/Program.cs)]

## Run the app

### [.NET Core CLI](#tab/netcore-cli)

```dotnetcli
dotnet run
```

### [Visual Studio](#tab/visual-studio)

Visual Studio uses an inconsistent working directory when running .NET Core console apps. (see [dotnet/project-system#3619](https://github.com/dotnet/project-system/issues/3619)) This results in an exception being thrown: *no such table: Blogs*. To update the working directory:

* Right-click on the project and select **Edit Project File**
* Just below the *TargetFramework* property, add the following:

  ``` XML
  <StartWorkingDirectory>$(MSBuildProjectDirectory)</StartWorkingDirectory>
  ```

* Save the file

Now you can run the app:

* **Debug > Start Without Debugging**

---

## Next steps

* Follow the [ASP.NET Core Tutorial](/aspnet/core/data/ef-rp/intro) to use EF Core in a web app
* Learn more about [LINQ query expressions](/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations)
* [Configure your model](xref:core/modeling/index) to specify things like [required](xref:core/modeling/entity-properties#required-and-optional-properties) and [maximum length](xref:core/modeling/entity-properties#maximum-length)
* Use [Migrations](xref:core/managing-schemas/migrations/index) to update the database schema after changing your model
