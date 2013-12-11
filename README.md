# unQuery

unQuery is a very minimal data access class that makes it as simple as possible to consume data and interact with SQL Server.

It is neither an OR/M nor an object mapper, and it does not intend to be either.

unQuery aims to ease the simpler use cases where strongly typed results are not necessary. I prefer SQL over query constructor abstractions, thus promoting the use of raw SQL.

## Access Methods

unQuery presents four different options for interacting with the database.

### Execute

Execute is used when you want to execute a batch but don't care about the results, other than the number of rows modified.

```csharp
int deletedUsers = DB.Execute("DELETE FROM Users WHERE Inactive = 1");
```

### GetScalar

GetScalar is used when you want to return the value of the first column in the first row of the batch.

```csharp
int numberOfUsers = DB.GetScalar<int>("SELECT COUNT(*) FROM Users");
```

### GetRow

GetRow is used when you want to return a single row of data. The result is a dynamic object with all the columns available as properties.

```csharp
var user = DB.GetRow("SELECT TOP 1 Name, Age FROM Users");

Console.WriteLine(user.Name + " (" + user.Age + " years old)");
```

### GetRows

GetRows is used when you want to return any number of rows of data. The result is a List of dynamic objects.

To avoid keeping the database hanging, GetRows will aggressively read all the data and return it as a List, rather than returning a lazily evaluated IEnumerable. GetRows ensures the schema is not replicated in each row, ensuring efficient storage of large amounts of data.

```csharp
var users = DB.GetRows("SELECT * FROM Users");

foreach (var user in users)
	Console.WriteLine(user.Name + " (" + user.Age + " years old)");
```

## Parameterization

All access methods support supplying an anonymous objects with parameters.

```csharp
var children = DB.GetRows("SELECT * FROM Users WHERE Age < @AdultThreshold", new {
	AdultThreshold = 18
});

var marks = DB.GetRow("SELECT TOP 1 * FROM Users WHERE Name = @Name", new {
	Name = Col.NVarChar("Mark", 64)
});

DB.Execute("INSERT INTO Users (Name, Age, Active) VALUES (@Name, @Age, @Active)", new {
	Name = Col.NVarChar("Mark", 64),
	Age = 28,
	Active = true
});
```

Simple C# types like byte, short, int, bigint, bool & Guid are automatically mapped to their equivalent database type. Ambiguous types like strings need to be mapped to a specific type using the Col factory. There are several ways of specifying the types explicitly.

```csharp
var row = DB.GetRow("SELECT @A, @B, @C", new {
	A = Col.VarChar("A"), // Length is based on the input
	B = Col.VarChar("A", 1), // Sets length explicitly for optimal plan reuse
	C = (SqlVarchar)"A" // Casts a string into a SqlVarChar with length based on the input
});
```

## Nulls

Nulls are handled automatically and translated ito & from DBNull.Value.

```csharp
DB.Execute("UPDATE Users SET Active = @Active WHERE UserID = @UserID", new {
	Active = (int?)null,
	UserID = 5
});

Assert.IsNull(DB.GetScalar<int?>("SELECT SUM(Age) FROM Users WHERE 1=0"));
```

## DB vs unQueryDB

The ```DB``` class is a static wrapper over a single instance of the unQueryDB class. If you only have one database, DB is the simplest way to go. Please see [Configuration](#configuration).

If you need to connect to multiple databases, you should use the unQueryDB class directly.

```csharp
var mainDB = new unQueryDB("Server=.;Database=MainDB;Trusted_Connection=True");
var secondaryDB = new unQueryDB("Server=.;Database=SecondaryDB;Trusted_Connection=True");

int mainUsers = mainDB.GetScalar<int>("SELECT COUNT(*) FROM Users");
int secondaryUsers = secondaryDB.GetScalar<int>("SELECT COUNT(*) FROM Users");
```

## Database Support

While unQuery currently requires no specific SQL Server functionality, I've decided to just support SQL Server. Just changing the use of SqlConnection to IDbConnection, SqlCommand to IDbComman and SqlParameter to IDbParameter might be enough, but I have not tested it.

I do foresee changing to a database agnostic implementation, but for now I want to keep it simple and open up if there's demand and people willing to test & support other platforms.

## Configuration

If you use the unQueryDB and pass a connection string to the constructor, no further configuration is necessary.

If you opt to use the static ```DB``` wrapper, unQuery will automatically use the first defined connection string in your configuration file.

Note that connections will inherit from your machine.config & root web.config files, so it may be safest to first clear existing connections and then define the one you want to connection.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<connectionStrings>
		<clear/>
		<add name="TestDB" connectionString="Server=.;Database=MainDB;Trusted_Connection=True"/>
	</connectionStrings>
</configuration>
```

## Contact

For any questions, issues or suggestions, please contact me at

* Mail: mark@improve.dk
* Twitter: @improvedk
* Blog: improve.dk