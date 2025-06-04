Use master
Go
-- Declare the database name
DECLARE @DatabaseName NVARCHAR(255) = 'ShiftItUpDB';

-- Generate and execute the kill commands for all active connections
DECLARE @KillCommand NVARCHAR(MAX);

SET @KillCommand = (
    SELECT STRING_AGG('KILL ' + CAST(session_id AS NVARCHAR), '; ')
    FROM sys.dm_exec_sessions
    WHERE database_id = DB_ID(@DatabaseName)
);

IF @KillCommand IS NOT NULL
BEGIN
    EXEC sp_executesql @KillCommand;
    PRINT 'All connections to the database have been terminated.';
END
ELSE
BEGIN
    PRINT 'No active connections to the database.';
END
Go

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'ShiftItUpDB')
BEGIN
    DROP DATABASE ShiftItUpDB;
END
Go

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'ShiftItUp')
BEGIN
    DROP DATABASE ShiftItUp;
END
Go
--Create Database ShiftItUpDB
--Go

--Use ShiftItUpDB
--Go

-- יצירת כניסה עבור המשתמש admin
CREATE LOGIN [ShiftAdminLogin] WITH PASSWORD = 'kukuPassword';
Go

ALTER SERVER ROLE sysadmin ADD MEMBER [ShiftAdminLogin];
Go
-- יצירת משתמש עבור ה-LOGIN במאגר הנתונים
--CREATE USER [ShiftAdminUser] FOR LOGIN [ShiftAdminLogin];
--Go

---- הוספת המשתמש לתפקיד db_owner
--ALTER ROLE db_owner ADD MEMBER [ShiftAdminUser];
--Go

use master
go

--Create Restore database command from backup with replace. The back up file is here:  C:\Users\User\Source\Repos\ShiftItUpServer\ShiftItUpServer\DBScripts\backup.bak
--School
RESTORE FILELISTONLY 
FROM DISK = 'C:\Users\User\Source\Repos\ShiftItUpServer\ShiftItUpServer\DBScripts\backup.bak';

RESTORE DATABASE ShiftItUpDB
FROM DISK = 'C:\Users\User\Source\Repos\ShiftItUpServer\ShiftItUpServer\DBScripts\backup.bak'
WITH 
    MOVE 'ShiftItUpDB' TO 'C:\Users\User\ShiftItUpDB.mdf',
    MOVE 'ShiftItUpDB_Log' TO 'C:\Users\User\ShiftItUpDB_log.ldf',
    REPLACE;
go


--Home
--RESTORE FILELISTONLY 
--FROM DISK = 'C:\Users\User\Source\Repos\ShiftItUpServer\ShiftItUpServer\DBScripts\backup.bak';

--RESTORE DATABASE ShiftItUpDB
--FROM DISK = 'C:\Users\User\Source\Repos\ShiftItUpServer\ShiftItUpServer\DBScripts\backup.bak'
--WITH 
--    MOVE 'ShiftItUpDB' TO 'C:\Users\User\ShiftItUpDB.mdf',
--    MOVE 'ShiftItUpDB_Log' TO 'C:\Users\User\ShiftItUpDB_log.ldf',
--    REPLACE;
--go


--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=ShiftItUpDB;User ID=ShiftAdminLogin;Password=kukuPassword;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context ShiftItUpDbContext -DataAnnotations -force
*/


