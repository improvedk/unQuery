# unQuery

Minimal generic data access layer. Work in progress - not ready for public use yet.

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

To avoid keeping the database hanging, GetRows will aggressively read all the data and return it as a List, rather than returning a lazily evaluated IEnumerable.

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

Nulls are handled automatically and translated into the proper DBNull.Value value.

```csharp
DB.Execute("UPDATE Users SET Active = @Active WHERE UserID = @UserID", new {
	Active = (int?)null,
	UserID = 5
});
```

## unQueryDB vs DB

## Database Support

## Configuration

* Adding ````<clear />```` to ConnectionStrings section as a precaution

## Contact

For any questions, issues or suggestions, please contact me at

* Mail: mark@improve.dk
* Twitter: @improvedk
* Blog: improve.dk