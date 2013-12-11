# unQuery

Minimal generic data access layer. Work in progress - not ready for public use yet.

## Methods

unQuery presents four different ways of interacting with the database.

### Execute

Execute is used when you want to execute a batch but don't care about the results, other than the number of rows modified.

    int deletedUsers = DB.Execute("DELETE FROM Users WHERE Inactive = 1");

### GetScalar

GetScalar is used when you want to return the value of the first column in the first row of the batch.

    int numberOfUsers = DB.GetScalar<int>("SELECT COUNT(*) FROM Users");

##TODO
====

* Adding ````<clear />```` to ConnectionStrings section as a precaution