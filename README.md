# unQuery

Minimal generic data access layer. Work in progress - not ready for public use yet.

## Methods

unQuery presents four different ways of interacting with the database.

### Execute

Execute is used when you want to execute a batch but don't care about the results, other than the number of rows modified.

    int deletedUsers = DB.Execute("DELETE FROM Users WHERE UserID = @UserID", new {
        UserID = 5
    });

### GetScalar

GetScalar is used when you want to return the value of the first column in the first row of the batch.

    int userID = DB.GetScalar<int>("INSERT INTO Users (Name) VALUES (@Name); SELECT SCOPE_IDENTITY()", new {
        Name = Col.NVarChar("Mark", 64)
    });

##TODO
====

* Adding ````<clear />```` to ConnectionStrings section as a precaution