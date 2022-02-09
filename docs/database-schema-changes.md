# Database Schema Changes
This document covers making changes to the database schema (how the database tables
are set up) in Uchu in development. Readng and writing to the database while running
is not covered.

## Setup
Part of the setup for pushing changes requires making database migrates, which defines
how a database migrates to new versions. To start, make sure you have `dotnet` in
your system PATH and either `python3` or `python` in your PATH. Which Python keyword
you use depends on your setup, but it will most likely be `python` on Windows and
`python3` on other systems. See external instructions for your operating system for
setting up .NET and Python.

With .NET installed, you will also need the Microsoft Entity Framework CLI tools.
This only needs to be run once if you don't have the CLI tools installed:
```bash
dotnet tool install --global dotnet-ef
```

## Making Schema Changes
Since Uchu uses Microsoft Entity Framework, changes to the database schema are done
in the C# classes. No matter what is done, database migrates will be required.

### Column Changes
The simpler of the two types of changes are to the columns of an existing table/class,
including adding or deleting (mostly adding - be careful deleting data for backward
compatibility). To do this, modify the database model class in [Uchu.Core/Database/Models](../Uchu.Core/Database/Models).
Properties in the models can also have attributes. Some examples include:
* `[Key]` - Marks the column as being the primary key. *1 and only 1 column must have this.*
* `[Required]` - Marks the column as being required, or `NOT NULL` in SQL.
* `[MaxLength(N)]` - For strings, this specifies the max length of strings as length N.

With Entity Framework, some types are able to be automatically handled, such as `DateTime`s
and references to other database models.

### New Models/Tables
Adding new database models/tables is more complicated since it requires creating the new
database model and setting it up with the database contexts. The steps to do this involve:
1. Create the new database model as a C# class in [Uchu.Core/Database/Models](../Uchu.Core/Database/Models).
   *Remember to specify the primary key with `[Key]`.*
2. Add the Model as a `DbSet<ClassName>` in [Uchu.Core/Database/Providers/UchuContextBase.cs](../Uchu.Core/Database/Providers/UchuContextBase.cs)
   similar to the other `DbSet<T>` properties.
3. Add the Model as a `DbSet<ClassName>` in [Uchu.Core/Database/UchuContext.cs](../Uchu.Core/Database/UchuContext.cs)
   similar to the other `DbSet<T>` properties.

## Creating Database Migrates
After the changes to the schema have been made, database migrates need to be made so the
database can be updated when Uchu starts to have the new columns or tables. Both ways
require a unique name for the migration. While it can be random, it is recommended to
be a human-readable name relevant to the change. In the commands below, this will be
`MigrateName`.

### Script (Recommended)
*If you are on macOS, this method is not supported since it involves downloading MariaDB,*
*which does not have downloads for macOS. Contributions are open for resolving this.*

A helper script exists for creating the database migrates for all the supported database
providers. The following can be run to generate the database migrates:
```bash
cd scripts
python3 CreateMigrates.py MigrateName
```

As mentioned before, `python3` may need to be replaced with `python`.

### Manually (Not Recommended)
Unlike SQLite and PostgreSQL, MySQL requires a running database. Setup on how to do
this is not covered and it is recommended to perform a proper secure setup.

For SQLite and PosgreSQL, the migrations can be created with the following:
```bash
cd Uchu.Core
dotnet ef migrations add SqliteMigrationName --context SqliteContext
dotnet ef migrations add PostgresMigrationName --context PostgresContext
```

The MySQL step is similar, but you will need to modify [Uchu.Core/Database/Providers/MySqlContext.cs](../Uchu.Core/Database/Providers/MySqlContext.cs)
temporarily with a valid connection string for your setup. **Make sure to
not commit this and to revert it.** After this, run the following:
```bash
dotnet ef migrations add MySqlMigrationName --context MySqlContext
```

## Commit Changes
After creating or modifying the database models, modifying the database contexts
(if applicable), and generating the migrates, the changes can be committed including
after trying to start Uchu. An error should occur if the migrates cause issues.