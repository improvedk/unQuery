# unQuery

unQuery is a very minimal data access library that makes it as simple as possible to consume data and interact with SQL Server.

It is neither an OR/M nor an object mapper, and it does not intend to be either.

## Access Methods

unQuery presents four different options for interacting with the database.

### GetScalar

GetScalar is used when you want to return the value of the first column in the first row of the batch.

```csharp
int numberOfUsers = DB.GetScalar<int>("SELECT COUNT(*) FROM Users");
```

If no matching row is found, a ```NoRowsException``` is thrown.

### GetRow

GetRow is used when you want to return a single row of data. The result is a dynamic object with all the columns available as properties.

```csharp
var user = DB.GetRow("SELECT TOP 1 Name, Age FROM Users");

Console.WriteLine(user.Name + " (" + user.Age + " years old)");
```

If no matching row is found, ```null``` is returned.

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

## Type Mapping

Both GetRow and GetRows support mapping the results into strong types. All you need to do is to provide the class as a generic parameter.

```csharp
public class Person
{
	public int PersonID { get; set; }
	public string Name { get; set; }
	public byte Age { get; set; }
	public string Sex { get; set; }
	public DateTime? SignedUp { get; set; }
}

// Gets a single Person
var lastSignup = DB.GetRow<Person>("SELECT TOP 1 * FROM Persons ORDER BY SignedUp DESC");

// Gets all females between 35 and 45
var femalesBetween35And45 = DB.GetRows<Person>("SELECT * FROM Persons WHERE Sex = @Sex AND Age BETWEEN @AgeFrom AND @AgeTo", new {
	Sex = "F",
	AgeFrom = 35,
	AgeTo = 45
});
```

### Notes
* Columns without a matching type property will be ignored.
* If property type does not match the column type, a TypeMismatchException will be thrown.
* If multiple columns share the same name, a DuplicateColumnException will be thrown.

### Simple Types
Besides mapping intotypes with properties, if all you're doing is selecting a list of a single column, you can map that directly into a list of that type.

```csharp
var userNames = DB.GetRows<string>("SELECT Name FROM Persons");
var lastLoginDates = DB.GetRows<DateTime?>("SELECT LastLoginDate FROM Persons");
```

If you happen to select more than one column, an exception will be thrown. This is to enforce you to only select the data you need, and to ensure you didn't intend to retrieve the other columns.

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
|:--------------|:---------------------:|:---------|
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

## Query Options

Sometimes you may need to tweak the way a query is run. All access methods take an optional parameter named `options` that allows you to set a number of options that modify the query.

```
// For example, if you need to set a command timeout
DB.Execute("DELETE FROM MyTable", options: new QueryOptions { CommandTimeout = 5 });

// And you can of course combine options with parameters
DB.Execute("DELETE FROM MyTable WHERE ID = @ID", new { ID = 25}, new QueryOptions {
	CommandTimeout = 5
});
```

For a full list of options, see the table below.

|Option|Type|Description|
|:-----|:--:|:----------|
|CommandTimeout|`int`|*Sets the query execution timeout in seconds.*|
|CommandType|`System.Data.CommandType`|*Sets the type of command to execute.*|

## Stored Procedures

All access methods support invoking stored procedures with input parameters. Output, InputOutput and ReturnValue parameters are not yet supported. All you need to do is to pass in a QueryOptions object with the `CommandType` property set to `CommandType.StoredProcedure`.

```
// Execute
DB.Execute("sp_rename", new {
	objname = "CurrentTableName",
	newname = "NewTableName"
}, new QueryOptions { CommandType: CommandType.StoredProcedure });

// GetScalar
var value = DB.GetScalar<int>("uspGetCustomerCount", options: new QueryOptions {
	CommandType: CommandType.StoredProcedure
});

// GetRow
var row = DB.GetRow("uspGetCustomerById", new {
	CustomerID = 123
}, new QueryOptions { CommandType: CommandType.StoredProcedure });

// GetRows
var rows = DB.GetRows("uspGetAllCustomers", options: new QueryOptions {
	CommandType: CommandType.StoredProcedure
});
```

## Batch Execution

If you have a single statement you want executed many times, with different parameter values, or if you have a series of distinct statements with unique parameters for each, unQuery allows you to execute batches efficiently.

```csharp
var persons = new[] {
	new { Name = "Mark", Age = 28, Human = true },
	new { Name = "Roger", Age = 35, Human = false }
};

using (var batch = DB.Batch())
{
	foreach (var person in persons)
		batch.Add("INSERT INTO Persons VALUES (@Name, @Age, @Human)", person);

	int numberOfRowsAffected = batch.Execute();
}
```

You can also execute completely distinct statements:

```csharp
using (var batch = DB.Batch())
{
	batch.Add("CREATE TABLE Test (ID int)");
	batch.Add("CREATE CLUSTERED INDEX CX_Test ON Test (ID ASC)");

	for (int i=0; i<100; i++)
		batch.Add("INSERT INTO Test VALUES (@I)", new { I = i });

	batch.Execute();
}
```

If you have a large number of rows to insert, with the same parameters, then table-valued parameters will be more efficient. But for those situations where you do not have a table-valued type available, Batch() can have a significant impact on performance.

If Execute() is not called, the batch will simply be disposed without touching the database.

### Atomicity & Performance

If any of the statements fail, Execute() will throw the corresponding SqlException. However, execution does not stop even if one statement fails. As such you could add statements A, B, C, D and E to the batch. If statements B and D fail, A, C and E will still be performed.

Even though using Batch() ensure that all of the statements are sent to SQL Server in a very efficient manner, each of the statements are still executed invidiually, as mentioned above. This also means that each statement will be committed by itself and thus result in an implicit transaction commit, causing SQL Server to write to the log. For optimum performance, and to ensure atomicity, you should wrap the Batch execution in a TransactionScope:

```csharp
using (var batch = DB.Batch())
{
	batch.Add("CREATE TABLE Test (ID int)");
	batch.Add("CREATE CLUSTERED INDEX CX_Test ON Test (ID ASC)");

	for (int i=0; i<100; i++)
		batch.Add("INSERT INTO Test VALUES (@I)", new { I = i });

	using (var ts = new TransactionScope())
	{
		batch.Execute();
		ts.Complete();
	}
}
```

Creating an ambient transaction through TransactionScope ensures that either all statements will succeed, or none will succeed. It also ensures that only one transaction commit is performed, meaning only one write is done to the log, by SQL Server.

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
* SQL Server 2008+ if you want to use table-valued parameters, otherwise SQL Server 2000+

## Contact

For any questions, issues or suggestions, please contact me at

* Mail: mark@improve.dk
* Twitter: @improvedk
* Blog: improve.dk
