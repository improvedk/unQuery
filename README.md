# unQuery

unQuery is a very minimal data access class that makes it as simple as possible to consume data and interact with SQL Server.

It is neither an OR/M nor an object mapper, and it does not intend to be either.

unQuery aims to ease the simpler use cases where strongly typed results are not necessary. I prefer SQL over query constructor abstractions, thus promoting the use of raw SQL.

## Access Methods

unQuery presents five different options for interacting with the database.

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

### Execute

Execute is used when you want to execute a batch but don't care about the results, other than the number of rows modified.

```csharp
int deletedUsers = DB.Execute("DELETE FROM Users WHERE Inactive = 1");
```

### ExecuteMany

If you have statement you want executed multiple times, though with different parameter types, and do not want the overhead of creating a table-valued type, ExecuteMany is a great option.

Say you want the numbers 1-100 inserted in a table, you could do the following:

```csharp
var rows = Enumerable.Range(1, 100).Select(x => new {
	Number = x,
	Even = x % 2
});
db.ExecuteMany("INSERT INTO Numbers (Number, Even) VALUES (@Number, @Even)", rows);
```

Rather than executing 100 individual SqlCommands, ExecuteMany will use the batch RPC API to efficiently send all 100 executions in one operation. Note that these will still run individually so you need to wrap the ExecuteMany call in a transaction to guarantee atomicity and to gain the full performance benefit:

```csharp
using (var ts = new TransactionScope())
{
	var rows = Enumerable.Range(1, 100).Select(x => new {
		Number = x,
		Even = x % 2
	});
	db.ExecuteMany("INSERT INTO Numbers (Number, Even) VALUES (@Number, @Even)", rows);
}
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

The table below shows which native .NET types can be automatically mapped to the equivalent SQL Server types, as well as how to use the Col factory class, for types that do not support auto-mapping.

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
|**nvarchar**|N/A|`Col.NVarChar(string value)`<br/>`Col.NVarChar(string value, int maxLength)`|
|**real**|N/A|`Col.Real(float? value)`|
|**smalldatetime**|N/A|`Col.SmallDateTime(DateTime? value)`|
|**smallint**|`short` / `short?`|`Col.SmallInt(short? value)`|
|**smallmoney**|N/A|`Col.SmallMoney(decimal? value)`|
|**text**|N/A|`Col.Text(string value)`|
|**time**|N/A|`Col.Time(TimeSpan? value)`<br/>`Col.Time(TimeSpan? value, byte scale)`|
|**tinyint**|`byte` / `byte?`|`Col.TinyInt(byte? value)`|
|**uniqueidentifier**|`Guid` / `Guid?`|`Col.UniqueIdentifier(Guid? value)`|
|**varbinary**|N/A|`Col.VarBinary(byte[] value)`<br/>`Col.VarBinary(byte[] value, int maxLength)`|
|**varchar**|N/A|`Col.VarChar(string value)`<br/>`Col.VarChar(string value, int maxLength)`|
|**xml**|N/A|`Col.Xml(string value)`|

While all types allow you to just specify a value and ignore the max length, precision and/or scale properties, it is **highly recommended** that you use the full constructors that specify all type properties. This allows SQL Server to reuse the execution plans across values, rather than compiling and caching a new plan for each combination of value properties that you send it.

```csharp
// This would cause two plans to be created and cached
DB.Execute("SELECT @Input", new { Input = Col.VarChar("Hello") });
DB.Execute("SELECT @Input", new { Input = Col.VarChar("Hello world") });

// Whereas this would result in just one plan
DB.Execute("SELECT @Input", new { Input = Col.VarChar("Hello", 50) });
DB.Execute("SELECT @Input", new { Input = Col.VarChar("Hello world", 50) });
```

Note that MAX types should have their length set to -1.

### Nulls

Nulls are handled automatically and translated to & from DBNull.Value.

```csharp
DB.Execute("UPDATE Users SET Active = @Active, Age = @Age WHERE UserID = @UserID", new {
	Active = (bool?)null,
	Age = Col.SmallInt(null),
	UserID = 5
});

Assert.IsNull(DB.GetScalar<int?>("SELECT SUM(Age) FROM Users WHERE 1=0"));
```

### Table-Valued Parameter Support

Tabled-Valued parameters are supported natively through the *Structured* parameter type. Before using table-valued parameters, a table type must be defined in the database. As an example, you might create a "Person" type like this:

```sql
CREATE TYPE MyPersonType AS Table (
	Name nvarchar(50) NOT NULL,
	Age smallint NOT NULL,
	Active bit NULL
)
```

Once you have the type, you can start using it immediately by creating a Structured parameter:

```csharp
var persons = DB.GetRows("SELECT * FROM @Persons", new {
	Persons = Col.Structured("MyPersonType", new [] {
		new { Name = Col.NVarChar("ABC", 50), Age = (short)25, Active = (bool?)true },
		new { Name = Col.NVarChar("XYZ", 50), Age = (short)2, Active = (bool?)false },
		new { Name = Col.NVarChar("IJK", 50), Age = (short)17, Active = (bool?)null }
	})
});
```

unQuery will create & cache code on-the-fly, for parsing in the parameters, so that performance is on par with handwritten code.

There are a couple of caveats when using structured parameters:

* Parameter types must always be fully specified - that is, varchars should have their max length defined, decimals should have their precision and scale defined, and so forth.
* The declaration order of the object properties must match the order of the SQL Server table type columns, exactly. That is (Name, Age, Active) in this case. If they're declared in a different order, an error will be thrown.

Due to the above caveats, I recommend you create a CLR type to match the SQL Server type, making it easier and more reliable to use. For example, the MyPersonType could be created like this:

```csharp
public class MyPersonType
{
	public SqlNVarChar Name { get; set; }
	public short Age { get; set; }
	public bool? Active { get; set; }

	public MyPersonType(string name, short age, bool? active)
	{
		Name = Col.NVarChar(name, 50);
		Age = age;
		Active = active;
	}
}
```

And once you have the type, using it with a structured parameter becomes much simpler and more eloquent:

```csharp
var persons = DB.GetRows("SELECT * FROM @Persons", new {
	Persons = Col.Structured("MyPersonType", new [] {
		new MyPersonType("ABC", 25, true),
		new MyPersonType("XYZ", 2, false),
		new MyPersonType("IJK", 17, null)
	})
});
```

#### Single-Column Table Types

If your table-type contains just one column, you can simply pass in an IEnumerable of that type. Say you wanted to pass in a list of integers, you could define the type like this:

```sql
CREATE TYPE ListOfInts AS Table (ID int NOT NULL)
```

And to invoke it you just need to pass in the values directly, typically in the form of an array, a List<T> or any other IEnumerable:

```csharp
var rows = db.GetRows("SELECT * FROM @Input", new {
	Input = Col.Structured("ListOfInts", new [] { 1, 2, 3 })
});
```

If you need to pass in types that can't be mapped directly from a CLR type, like ints, you can also pass in an IEnumerable ISqlType values like so:

```csharp
var rows = db.GetRows("SELECT * FROM @Input", new {
	Input = Col.Structured("ListOfSmallMoneys", new [] {
		Col.SmallMoney(5.27m),
		Col.SmallMoney(8.32m)
	})
});
```

One word of caution - if the parameter needs specifications, like a decimal value, the specs on the first value will set the specification for all of the remaining values. An example:

```csharp
var rows = db.GetRows("SELECT * FROM @Input", new {
	Input = Col.Structured("ListOfDecimals", new [] {
		Col.Decimal(5.27m, 3, 2),

		// While this won't throw an exception, the column values will be in the form of
		// decimal(3, 2) as the first value sets the presedence.
		Col.Decimal(8.32m, 5, 4)
	})
});
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

## Requirements

* .NET Framework 4.0+
* SQL Server 2008+

## Contact

For any questions, issues or suggestions, please contact me at

* Mail: mark@improve.dk
* Twitter: @improvedk
* Blog: improve.dk
