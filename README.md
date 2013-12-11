# unQuery

Minimal generic data access layer. Work in progress - not ready for public use yet.

## Methods

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

##TODO
====

* Adding ````<clear />```` to ConnectionStrings section as a precaution