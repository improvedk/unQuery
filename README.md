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

To avoid keeping the database hanging, GetRows will aggressively read all the data and return it as a List, rather than returning a lazily evaluated IEnumerable. GetRows ensures the schema is stored just once, rather than in each row, ensuring efficient handling of large amounts of rows.

```csharp
var users = DB.GetRows("SELECT * FROM Users");

foreach (var user in users)
	Console.WriteLine(user.Name + " (" + user.Age + " years old)");
```

### SqlCommand Overloads

Each access method supports the use of raw SQL, while also allowing you to pass in a SqlCommand. Creating your own SqlCommand can be useful if you need custom configuration or full control over the parameterization.

```csharp
// These two are functionally equivalent
int deletedUsers = DB.Execute(new SqlCommand("DELETE FROM Users"));
int deletedUsers = DB.Execute("DELETE FROM Users");
```

You may also add parameters to an already parameterized SqlCommand.

```csharp
var cmd = new SqlCommand("DELETE FROM Users WHERE Age > @Age AND Active = @Active");
cmd.Parameters.Add("@Age", SqlDbType.Int).Value = 50;

int deletedUsers = DB.Execute(cmd, new {
	Active = false
});
```

## Parameterization

All access methods support supplying an anonymous objects with parameters.

```csharp
var children = DB.GetRows("SELECT * FROM Users WHERE Age < @AdultThreshold", new {
	AdultThreshold = 18 // int
});

var marks = DB.GetRow("SELECT TOP 1 * FROM Users WHERE Name = @Name", new {
	Name = Col.NVarChar("Mark", 64) // nvarchar(64)
});

DB.Execute("INSERT INTO Users (Name, Age, Active, Comments) VALUES (@Name, @Age, @Active, @Comments)", new {
	Name = Col.NVarChar("Mark", 64), // nvarchar(64)
	Age = 28, // int
	Active = true, // bit
	Comments = Col.NVarChar(null, -1) // nvarchar(MAX)
});
```

### Type Support

unQuery supports almost all of the built-in types in SQL Server. Types that can be mapped between .NET and SQL Server automatically are supported as implicit types, whereas all others require you to use the `Col` factory for creating parameter values.

The table below shows which native .NET types can be automatically mapped to the equivanlent SQL Server types, as well as how to use the Col factory class, for types that do not support auto-mapping.

Note that MAX types should have their length set to -1.

|SQL Server Type|Implicit .NET Type (C#)|Col Syntax|
|---------------|:---------------------:|:---------|
|**bigint**|`long` / `long?`|`Col.BigInt(long? value)`|
|**binary**|N/A|`Col.Binary(byte[] value)`<br/>`Col.Binary(byte[] value, int maxLength)`|
|**bit**|`bool` / `bool?`|`Col.Bit(bool? value)`|
|**char**|N/A|`Col.Char(string value)`<br/>`Col.Char(string value, int maxLength)`|
|**date**|N/A|`Col.Date(DateTime? value)`|
|**datetime**|N/A|`Col.DateTime(DateTime? value)`|
|**datetime2**|N/A|`Col.DateTime2(DateTime? value)`<br/>`Col.DateTime2(DateTime? value, byte scale)`|
|**datetimeoffset**|N/A|`Col.DateTimeOffset(DateTimeOffset? value)`<br/>`Col.DateTimeOffset(DateTimeOffset? value, byte scale)`|
|**decimal**|N/A|`Col.Decimal(decimal? value)`<br/>`Col.Decimal(decimal? value, byte precision, byte scale)`|
|**float**|N/A|`Col.Float(double? value)`|
|**image**|N/A|`Col.Image(byte[] value)`|
|**int**|`int` / `int?`|`Col.Int(int? value)`|
|**money**|N/A|`Col.Money(decimal? value)`|
|**nchar**|N/A|`Col.NChar(string value)`<br/>`Col.NChar(string value, int maxLength)`|
|**ntext**|N/A|`Col.NText(string value)`|
|**numeric**|N/A|`Col.Decimal(decimal? value)`<br/>`Col.Decimal(decimal? value, byte precision, byte scale)`|
|**nvarchar**|N/A|`Col.NVarChar(string value, int maxLength)`|
|**real**|N/A|`Col.Real(float? value)`|
|**smalldatetime**|N/A|`Col.SmallDateTime(DateTime? value)`|
|**smallint**|`short` / `short?`|`Col.SmallInt(short? value)`|
|**smallmoney**|N/A|`Col.SmallMoney(decimal? value)`|
|**text**|N/A|`Col.Text(string value)`|
|**time**|N/A|`Col.Time(TimeSpan? value, byte scale)`|
|**tinyint**|`byte` / `byte?`|`Col.TinyInt(byte? value)`|
|**uniqueidentifier**|`Guid` / `Guid?`|`Col.UniqueIdentifier(Guid? value)`|
|**varbinary**|N/A|`Col.VarBinary(byte[] value, int maxLength)`|
|**varchar**|N/A|`Col.VarChar(string value, int maxLength)`|
|**xml**|N/A|`Col.Xml(string value)`|

## Nulls

Nulls are handled automatically and translated to & from DBNull.Value.

```csharp
DB.Execute("UPDATE Users SET Active = @Active, Age = @Age WHERE UserID = @UserID", new {
	Active = (bool?)null,
	Age = Col.SmallInt(null),
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
