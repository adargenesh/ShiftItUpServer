Use master
Go

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'ShiftItUpDB')
BEGIN
    DROP DATABASE ShiftItUpDB;
END
Go

Create Database ShiftItUpDB
Go

Use ShiftItUpDB
Go

-- יצירת טבלאות
Create Table Store
(
    IdStore int Primary Key identity,
    StoreName nvarchar(50) Not Null,
    StoreAdress nvarchar(50) Not Null,
    StoreManager nvarchar(50) Not Null,
    ManagerEmail nvarchar(50)  Unique Not Null,
    ManagerPassword nvarchar(50) Not Null
);

Create Table [Status]
(
    Id int Primary Key,
    [Name] nvarchar(50) Not Null
);

Create Table Worker
(
    WorkerId int Primary Key Identity,
    UserName nvarchar(50) Not Null,
    UserLastName nvarchar(50) Not Null,
    UserEmail nvarchar(50) Unique Not Null,
    UserPassword nvarchar(50) Not Null,
    IdStore int Not Null,
    UserSalary nvarchar(50) Not Null,
    StatusWorker int Not Null,
    Foreign key (IdStore) References Store (IdStore),
    Foreign key (StatusWorker) References [Status] (Id)
);

Create Table [Shift]
(
    ShiftID int Primary Key Identity,
    ShiftStart datetime Not Null,
    ShiftEnd datetime Not Null,
    SalesGoal decimal Not Null,
    SalesActual decimal Null,
    NumEmployees int not null
);

Create Table [WorkerInShift]
(
    ShiftID int not null,
    WorkerId int Not Null,
    SystemRemarks nvarchar(500) null,
    Foreign key (ShiftID) References [Shift] (ShiftID),
    Foreign key (WorkerId) References Worker (WorkerId)
);


Create Table [WorkerShiftRequest]
(
    RequestID int identity primary key,
    WeekNum int not null,
    [Year] int not null,
    WorkerId int Not Null,
    Remarks nvarchar(500) not  null,
 
    Foreign key (WorkerId) References Worker (WorkerId)
);

Create Table [DefiningShift]
(
    DefiningShiftID int Primary Key Identity,
    IdStore int not null,
    Foreign key (IdStore) References Store (IdStore),
    [DayOfWeek] int not null, 
    StartTime time Not Null,
    EndTime time Not Null,
    NumEmployees int not null
);

Go

-- הוספת סטטוסים לטבלה [Status]
INSERT INTO [Status] VALUES (0, N'Approved');
INSERT INTO [Status] VALUES (1, N'Declined');
INSERT INTO [Status] VALUES (2, N'Pending');
Go

-- קודם כל, הוסף את הנתונים לטבלה Store, ואז Worker
insert into Store (StoreName, StoreAdress, StoreManager, ManagerEmail, ManagerPassword)
values ('Billa', 'Street 123', 'Adi', 'adi@billa.com', 'password123');
insert into Store (StoreName, StoreAdress, StoreManager, ManagerEmail, ManagerPassword)
values ('HgDESIGN', 'Street 16', 'HADAS', 'HADAS@HGDESIGHN.com', 'HG123');


-- עכשיו הוסף את הנתונים לטבלת Worker, כאשר ה-IdStore שייך לערך קיים בטבלת Store (במקרה זה 1)
insert into Worker (UserName, UserLastName, UserEmail, UserPassword, IdStore, UserSalary, StatusWorker)
values ('Adar', 'Genesh', 'adar.g@gmail.com', 'adar123',  1, '5000', 0);

insert into Worker (UserName, UserLastName, UserEmail, UserPassword, IdStore, UserSalary, StatusWorker)
values ('Adar2', 'Genesh', 'adar2.g@gmail.com', 'adar2123',  1, '5000', 2);

insert into Worker (UserName, UserLastName, UserEmail, UserPassword, IdStore, UserSalary, StatusWorker)
values ('Adar3', 'Genesh', 'adar3.g@gmail.com', 'adar3123',  1, '5000', 2);

insert into Worker (UserName, UserLastName, UserEmail, UserPassword, IdStore, UserSalary, StatusWorker)
values ('Adar4', 'Genesh', 'adar4.g@gmail.com', 'adar4123',  1, '5000', 1);
insert into Worker (UserName, UserLastName, UserEmail, UserPassword, IdStore, UserSalary, StatusWorker)
values ('AMIT', 'BBB', 'AMIT.g@gmail.com', 'AMIT123',  2, '100000', 0);



-- יצירת כניסה עבור המשתמש admin
CREATE LOGIN [ShiftAdminLogin] WITH PASSWORD = 'kukuPassword';
Go

-- יצירת משתמש עבור ה-LOGIN במאגר הנתונים
CREATE USER [ShiftAdminUser] FOR LOGIN [ShiftAdminLogin];
Go

-- הוספת המשתמש לתפקיד db_owner
ALTER ROLE db_owner ADD MEMBER [ShiftAdminUser];
Go

--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=ShiftItUpDB;User ID=ShiftAdminLogin;Password=kukuPassword;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context ShiftItUpDbContext -DataAnnotations -force
*/

SELECT * FROM [Worker]
GO
select * from Store
select * from DefiningShift
select *from [Status] 
select *from WorkerShiftRequest
DELETE [Worker] WHERE WorkerId=5;