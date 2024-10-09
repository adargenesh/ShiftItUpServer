Create Database ShiftItUpDB

Go

Use ShiftItUpDB

Go

Create Table Store
(
IdStore int Primary Key,
StoreName nvarchar(50) Not Null,
StoreAdress nvarchar(50) Not Null,
StoreManager nvarchar(50) Not Null,
ManagerEmail nvarchar(50)  Unique Not Null
);
Create Table [Status]
(
Id int Primary Key Identity,
[Name] nvarchar(50) Not Null,
);

Create Table Worker
(
WorkerId int Primary Key Identity,
UserName nvarchar(50) Not Null,
UserLastName nvarchar(50) Not Null,
UserEmail nvarchar(50) Unique Not Null,
UserPassword nvarchar(50) Not Null,
UserStoreName nvarchar(50) Not Null,
IdStoreManager nvarchar(50) Not Null,
UserSalary nvarchar(50) Not Null,
StatusWorker nvarchar(50) Not Null,
Foreign key (IdStoreManager ) References Store (IdStore) ,
Foreign key (StatusWorker ) References [Status] (Id) 
);

Create Table [Shift]
(
ShiftID int Primary Key Identity,
ShiftDate date Not Null,
ShiftGoal nvarchar(50) Not Null,
);

Create Table [WorkerInShift]
(
ShiftID nvarchar(50) Not Null,
WorkerId nvarchar(50) Not Null,
Foreign key (ShiftID ) References [Shift] (ShiftID),
Foreign key (WorkerId ) References Worker (WorkerId) 
);

Create Table [DefiningShift]
(
ShiftID nvarchar(50) Not Null,
ShiftDate date Not Null,
ShiftHour nvarchar(50) Not Null,
);



