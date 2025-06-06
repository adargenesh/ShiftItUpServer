﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShiftItUpServer.DTO;
using ShiftItUpServer.Models;
using ShiftItUpServer.Services;

[Route("api")]
[ApiController]
public class ShiftItUpAPIController : ControllerBase
{
    //a variable to hold a reference to the db context!
    private ShiftItUpDbContext context;
    //a variable that hold a reference to web hosting interface (that provide information like the folder on which the server runs etc...)
    private IWebHostEnvironment webHostEnvironment;
    //Use dependency injection to get the db context and web host into the constructor
    public ShiftItUpAPIController(ShiftItUpDbContext context, IWebHostEnvironment env)
    {
        this.context = context;
        this.webHostEnvironment = env;
    }

    [HttpGet]
    [Route("TestServer")]
    public ActionResult<string> TestServer()
    {
        return Ok("Server Responded Successfully");
    }

    [HttpPost("loginWorker")]
    public IActionResult LoginWorker([FromBody] ShiftItUpServer.DTO.LoginDto loginDto)
    {
        try
        {
            HttpContext.Session.Clear(); //Logout any previous login attempt

            //Get model user class from DB with matching email. 
            ShiftItUpServer.Models.Worker? modelsUser = context.GetUser(loginDto.UserEmail);

            //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
            if (modelsUser == null || modelsUser.UserPassword != loginDto.UserPassword || modelsUser.StatusWorker != 0)
            {
                return Unauthorized();
            }

            //Login suceed! now mark login in session memory!
            HttpContext.Session.SetString("loggedInUser", modelsUser.UserEmail);

            ShiftItUpServer.DTO.WorkerDto dtoUser = new ShiftItUpServer.DTO.WorkerDto(modelsUser);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.WorkerId, false);
            return Ok(dtoUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("loginStore")]
    public IActionResult LoginStore([FromBody] ShiftItUpServer.DTO.LoginDto loginDto)
    {
        try
        {
            HttpContext.Session.Clear(); //Logout any previous login attempt

            //Get model store class from DB with matching email. 
            ShiftItUpServer.Models.Store? modelsStore = context.GetStore(loginDto.UserEmail);

            //Check if store exist for this email and if password match, if not return Access Denied (Error 403) 
            if (modelsStore == null || modelsStore.ManagerPassword != loginDto.UserPassword)
            {
                return Unauthorized();
            }

            //Login suceed! now mark login in session memory!
            HttpContext.Session.SetString("loggedInStore", modelsStore.ManagerEmail);
            

            ShiftItUpServer.DTO.StoreDto dtoStore = new ShiftItUpServer.DTO.StoreDto(modelsStore);
            dtoStore.ProfileImagePath = GetProfileImageVirtualPath(dtoStore.IdStore, true);
            return Ok(dtoStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("registerWorker")]
    public IActionResult RegisterWorker([FromBody] ShiftItUpServer.DTO.WorkerDto userDto)
    {
        try
        {
            HttpContext.Session.Clear(); //Logout any previous login attempt

            //Create model user class
            ShiftItUpServer.Models.Worker modelsUser = userDto.GetModel();

            context.Workers.Add(modelsUser);
            context.SaveChanges();

            //User was added!
            ShiftItUpServer.DTO.WorkerDto dtoUser = new ShiftItUpServer.DTO.WorkerDto(modelsUser);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.WorkerId, false);
            return Ok(dtoUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("AddShiftRequest")]
    public IActionResult AddShiftRequest([FromBody] WorkerShiftRequestDto request)
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            //Create model user class
            ShiftItUpServer.Models.WorkerShiftRequest modelRequest = request.GetModel();

            context.WorkerShiftRequests.Add(modelRequest);
            context.SaveChanges();

            //User was added!
            ShiftItUpServer.DTO.WorkerShiftRequestDto dto = new WorkerShiftRequestDto(modelRequest);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("registerStore")]
    public IActionResult RegisterStore([FromBody] ShiftItUpServer.DTO.StoreDto storeDto)
    {
        try
        {
            HttpContext.Session.Clear(); //Logout any previous login attempt

            //Create model store class
            ShiftItUpServer.Models.Store modelsStore = storeDto.GetModel();
            //Add default defining shifts that will be changed later if necessary
            modelsStore.DefiningShifts = new List<DefiningShift>();
            for(int dayOfWeek = 1; dayOfWeek < 8; dayOfWeek++)
            {
                modelsStore.DefiningShifts.Add(new DefiningShift()
                {
                    IdStore = 0,
                    DayOfWeek = dayOfWeek,
                    NumEmployees = 2,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(16, 0)
                });
                modelsStore.DefiningShifts.Add(new DefiningShift()
                {
                    IdStore = 0,
                    DayOfWeek = dayOfWeek,
                    NumEmployees = 2,
                    StartTime = new TimeOnly(16, 0),
                    EndTime = new TimeOnly(22, 0)
                });
            }
            
            context.Stores.Add(modelsStore);
            context.SaveChanges();

            //store was added!
            ShiftItUpServer.DTO.StoreDto dtoStore = new ShiftItUpServer.DTO.StoreDto(modelsStore);
            dtoStore.ProfileImagePath = GetProfileImageVirtualPath(dtoStore.IdStore, true);
            return Ok(dtoStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("getStores")]
    public IActionResult GetStores()
    {
        try
        {
            
            //Read stores from database
            List<Store> stores = context.Stores.ToList();
            List<StoreDto> dtoStores = new List<StoreDto>();
            foreach (Store store in stores)
            {
                dtoStores.Add(new StoreDto(store));
            }
            return Ok(dtoStores);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("getStatuses")]
    public IActionResult GetStatuses()
    {
        try
        {

            //Read stores from database
            List<Status> list = context.Statuses.ToList();
            List<StatusDto> dtoStatuses = new List<StatusDto>();
            foreach (Status s in list)
            {
                dtoStatuses.Add(new StatusDto(s));
            }
            return Ok(dtoStatuses);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("getWorkersOfStore")]
    public IActionResult GetWorkersOfStore()
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if(string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            //Extract store id of the loged in user
            int storeid;
            if (IsStoreLoggedin())
            {
                Store? s = context.GetStore(email);
                storeid = s.IdStore;
            }
            else
            {
                Worker? w = context.GetUser(email);
                storeid = w.IdStore;
            }

            //Read stores from database
            List<Worker> workers = context.Workers.Where(w=>w.IdStore==storeid).ToList();
            List<WorkerDto> dtoWorkers = new List<WorkerDto>();
            foreach (Worker worker in workers)
            {
                WorkerDto w = new WorkerDto(worker);
                w.ProfileImagePath = GetProfileImageVirtualPath(w.WorkerId, false);
                dtoWorkers.Add(w);
            }
            return Ok(dtoWorkers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    private string? GetLoggedInEmail()
    {
        string? email = HttpContext.Session.GetString("loggedInUser");
        if (string.IsNullOrEmpty(email))
        {
            email = HttpContext.Session.GetString("loggedInStore");
        }
        return email;

    }

    private bool IsStoreLoggedin()
    {
        string? email = HttpContext.Session.GetString("loggedInStore");
        return !string.IsNullOrEmpty(email);

    }

    private bool IsStore()
    {
        string? email = HttpContext.Session.GetString("loggedInUser");
        if (string.IsNullOrEmpty(email))
        {
            return true;
        }
        return false;

    }

    [HttpPost("UploadProfileImage")]
    public async Task<IActionResult> UploadProfileImageAsync(IFormFile file)
    {
        //Check if who is logged in
        string? email = GetLoggedInEmail();

        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User is not logged in");
        }

        bool isStore = IsStore();

        string path = "";

        Object loggedInObject = null;
        if (isStore)
        {
            //Get model user class from DB with matching email. 
            ShiftItUpServer.Models.Store? s = context.GetStore(email);
            loggedInObject = s;
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();
            path = $"\\storeImages\\{s.IdStore}";
        }
        else
        {
            //Get model user class from DB with matching email. 
            ShiftItUpServer.Models.Worker? w = context.GetUser(email);
            loggedInObject = w;
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();
            path = $"\\profileImages\\{w.WorkerId}";
        }

        

        //Read all files sent
        long imagesSize = 0;

        if (file.Length > 0)
        {
            //Check the file extention!
            string[] allowedExtentions = { ".png", ".jpg" };
            string extention = "";
            if (file.FileName.LastIndexOf(".") > 0)
            {
                extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
            }
            if (!allowedExtentions.Where(e => e == extention).Any())
            {
                //Extention is not supported
                return BadRequest("File sent with non supported extention");
            }

            //Build path in the web root (better to a specific folder under the web root
            string filePath = $"{this.webHostEnvironment.WebRootPath}{path}{extention}";

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);

                if (IsImage(stream))
                {
                    imagesSize += stream.Length;
                }
                else
                {
                    //Delete the file if it is not supported!
                    System.IO.File.Delete(filePath);
                }

            }

        }

        if (isStore)
        {
            ShiftItUpServer.DTO.StoreDto dtoStore = new ShiftItUpServer.DTO.StoreDto((Store)loggedInObject);
            dtoStore.ProfileImagePath = GetProfileImageVirtualPath(dtoStore.IdStore, true);
            return Ok(dtoStore);
        }
        else
        {
            ShiftItUpServer.DTO.WorkerDto dtoUser = new ShiftItUpServer.DTO.WorkerDto((Worker)loggedInObject);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.WorkerId, false);
            return Ok(dtoUser);
        }
        
    }

    //Helper functions

    //this function gets a file stream and check if it is an image
    private static bool IsImage(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        List<string> jpg = new List<string> { "FF", "D8" };
        List<string> bmp = new List<string> { "42", "4D" };
        List<string> gif = new List<string> { "47", "49", "46" };
        List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
        List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

        List<string> bytesIterated = new List<string>();

        for (int i = 0; i < 8; i++)
        {
            string bit = stream.ReadByte().ToString("X2");
            bytesIterated.Add(bit);

            bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
            if (isImage)
            {
                return true;
            }
        }

        return false;
    }

    [HttpPost("ApproveWorker")]
    public IActionResult ApproveWorker([FromBody] ShiftItUpServer.DTO.WorkerDto userDto)
    {
        //First update user in database
        IActionResult actionResult = UpdateWorker(userDto);
        //Send email to the worker to notify him that he is approved
        Store? s = context.Stores.Where(ss => ss.IdStore == userDto.IdStore).FirstOrDefault();
        if (s != null)
        {
            SendApproveEmailToWorker(userDto.UserEmail, userDto.UserName, s.StoreName, userDto.UserSalary);
        }
        return actionResult;
    }

    [HttpPost("updateWorker")]
    public IActionResult UpdateWorker([FromBody] ShiftItUpServer.DTO.WorkerDto userDto)
    {
        try
        {
            string? email = GetLoggedInEmail();
            if (email == null)
            {
                return Unauthorized();
            }

            //Create model user class
            ShiftItUpServer.Models.Worker modelsUser = userDto.GetModel();

            context.Workers.Update(modelsUser);
            context.SaveChanges();

            //User was added!
            ShiftItUpServer.DTO.WorkerDto dtoUser = new ShiftItUpServer.DTO.WorkerDto(modelsUser);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.WorkerId, false);
            return Ok(dtoUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    private async void SendApproveEmailToWorker(string workerEmail, string workerName, string storeName, string salary)
    {
        EmailData email = new EmailData()
        {
            From = "ShiftItUp System",
            To = workerEmail,
            Subject = "Welcome To ShiftItUp System!",
            Body = $"Dear {workerName}, We are happy to inform you that you got accepted to {storeName}\nYour salary will be {salary} NIS per hour\nGoiod luck!"
        };

        SendEmailService sendEmailService = new SendEmailService();
        await sendEmailService.Send(email);

    }













    private string GetProfileImageVirtualPath(int id, bool isStore)
    {
        string localPath = $"\\profileImages\\{id}";
        string virtualPath = $"/profileImages/{id}";
        if ( (isStore))
        {
            virtualPath = $"/storeImages/{id}";
            localPath = $"\\storeImages\\{id}";
        }
        string path = $"{this.webHostEnvironment.WebRootPath}{localPath}.png";
        if (System.IO.File.Exists(path))
        {
            virtualPath += ".png";
        }
        else
        {
            path = $"{this.webHostEnvironment.WebRootPath}{localPath}.jpg";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".jpg";
            }
            else
            {
                if (!isStore)
                    virtualPath = $"/profileImages/default.png";
                else
                    virtualPath = $"/storeImages/default.png";
            }
        }

        return virtualPath;
    }

    [HttpPost("updateprofilestore")]
    public async Task<IActionResult> UpdateProfileStore([FromBody] StoreDto storeDto)
    {
        if (storeDto == null)
        {
            return BadRequest("Store data is null");
        }

        // חיפוש המשתמש לפי Id
        var store = await context.Stores.FindAsync(storeDto.IdStore);

        if (store == null)
        {
            return NotFound($"Store with ID {storeDto.IdStore} not found");
        }

        // עדכון השדות של המשתמש
        store.StoreManager = storeDto.StoreManager;
        store.StoreName = storeDto.StoreName;
        store.StoreAdress = storeDto.StoreAddress;
        store.ManagerEmail = storeDto.ManagerEmail;
        store.ManagerPassword = storeDto.ManagerPassword;

        try
        {
            // שמירת השינויים למסד הנתונים
            await context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (Exception ex)
        {
            // טיפול בשגיאות
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", error = ex.Message });
        }
    }


    [HttpPost("updateprofile")]
    public async Task<IActionResult> UpdateProfile([FromBody] WorkerDto userDto)
    {
        if (userDto == null)
        {
            return BadRequest("User data is null");
        }

        // חיפוש המשתמש לפי Id
        var user = await context.Workers.FindAsync(userDto.WorkerId);

        if (user == null)
        {
            return NotFound($"User with ID {userDto.WorkerId} not found");
        }

        // עדכון השדות של המשתמש
        user.UserName = userDto.UserName;
        user.UserLastName = userDto.UserLastName;
        user.UserEmail = userDto.UserEmail;
        user.UserPassword = userDto.UserPassword;
        user.UserSalary = userDto.UserSalary;   

        try
        {
            // שמירת השינויים למסד הנתונים
            await context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (Exception ex)
        {
            // טיפול בשגיאות
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpGet("GetAllWorkers")]
    public IActionResult GetAllWorkers()
    {
        //validate later
        try
        {
            List<Worker> list = context.GetAllWorkers();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }


    [HttpGet("getWorkersOfStoreByStatus")]
    public IActionResult GetWorkersOfStoreByStatus([FromQuery] int statusWorker)
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            //Extract store id of the loged in user
            int storeid;
            if (IsStoreLoggedin())
            {
                Store? s = context.GetStore(email);
                storeid = s.IdStore;
            }
            else
            {
                Worker? w = context.GetUser(email);
                storeid = w.IdStore;
            }

            //Read stores from database
            List<Worker> workers = context.Workers.Where(w => w.IdStore == storeid && w.StatusWorker== statusWorker).ToList();
            List<WorkerDto> dtoWorkers = new List<WorkerDto>();
            foreach (Worker worker in workers)
            {
                WorkerDto w = new WorkerDto(worker);
                w.ProfileImagePath = GetProfileImageVirtualPath(w.WorkerId, false);
                dtoWorkers.Add(w);
            }
            return Ok(dtoWorkers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    [HttpGet("getAllRequestOfStore")]
    public IActionResult GetAllRequestOfStore()
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            //Extract store id of the loged in user
            int storeid;
            if (IsStoreLoggedin())
            {
                Store? s = context.GetStore(email);
                storeid = s.IdStore;
            }
            else
            {
                Worker? w = context.GetUser(email);
                storeid = w.IdStore;
            }

            //Read stores from database
            Store? store = context.Stores.Where(w => w.IdStore == storeid).
                                                Include(s => s.Workers).
                                                ThenInclude(w => w.WorkerShiftRequests).FirstOrDefault();
            List<WorkerShiftRequestDto> list = new List<WorkerShiftRequestDto>();

            foreach(Worker w in store.Workers)
            {
                foreach(WorkerShiftRequest r in w.WorkerShiftRequests)
                {
                    WorkerShiftRequestDto dto = new WorkerShiftRequestDto(r);
                    dto.Worker.ProfileImagePath = GetProfileImageVirtualPath(dto.WorkerId, false);
                    list.Add(dto);
                    
                }
            }
            

            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("GetDefiningShifts")]
    public IActionResult GetDefiningShifts()
    {
        string? email = GetLoggedInEmail();
        if (email == null)
        {
            return Unauthorized();
        }

        Store? store = context.GetStore(email);
        if (store == null)
        {
            return NotFound("Store not found");
        }

        List<DefiningShiftDto> definingShiftDtos = new List<DefiningShiftDto>();

        foreach(DefiningShift definingShift in store.DefiningShifts)
        {
            DefiningShiftDto definingShiftDto = new DefiningShiftDto(definingShift);
            definingShiftDtos.Add(definingShiftDto);
        }

        return Ok(definingShiftDtos);

    }

    [HttpPost("UpdateDefiningShifts")]
    public IActionResult UpdateDefiningShifts(List<DefiningShiftDto> shifts)
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (email == null)
            {
                return Unauthorized();
            }

            //Check if user is logged a store
            Store? store = context.GetStore(email);
            if (store == null)
            {
                return NotFound("Store not found");
            }

            //Check if the shifts belong to the store
            foreach (DefiningShiftDto definingShiftDto in shifts)
            {
                if (definingShiftDto.IdStore != store.IdStore)
                {
                    return BadRequest("Defining shift does not belong to the store");
                }
            }

            //check if there are shifts that need to be removed from the database
            List<DefiningShift> definingShifts = store.DefiningShifts.ToList();
            foreach (DefiningShift definingShift in definingShifts)
            {
                if (!shifts.Any(s => s.DefiningShiftId == definingShift.DefiningShiftId))
                {
                    context.DefiningShifts.Remove(definingShift);
                }
            }
            context.SaveChanges();

            //Update all shifts in the database
            context.ChangeTracker.Clear(); //Clear the tracking of all objects to avoid double tracking
            definingShifts = new List<DefiningShift>();

            foreach (DefiningShiftDto definingShift in shifts)
            {
                DefiningShift modelShift = definingShift.GetModel();
                definingShifts.Add(modelShift);
            }

            context.DefiningShifts.UpdateRange(definingShifts);
            context.SaveChanges();
            return Ok();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("AddShiftSchedule")]
    public IActionResult AddShiftSchedule(List<ShiftDto> shifts)
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (email == null)
            {
                return Unauthorized();
            }

            //Check if user is logged a store
            Store? store = context.GetStore(email);
            if (store == null)
            {
                return NotFound("Store not found");
            }

            
            //Save shifts in database
            List<Shift> dbShifts = new List<Shift>();
            foreach (ShiftDto s in shifts)
            {
                Shift shift = s.GetModels();
                context.Shifts.Update(shift);
            }
            context.SaveChanges();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("ReadShiftSchedule")]
    public IActionResult ReadShiftSchedule([FromBody] DateTime start)
    {
        try
        {
            //Check if user is logged in
            string? email = GetLoggedInEmail();
            if (email == null)
            {
                return Unauthorized();
            }

            //Save shifts in database
            List<Shift> dbShifts = context.Shifts
                                           .Include(s=>s.WorkerInShifts).ThenInclude(w=>w.Worker)
                                           .Where(s => s.ShiftStart >= start && s.ShiftStart < start.AddDays(7)).ToList();
            List<ShiftDto> shifts = new List<ShiftDto>();
            foreach (Shift s in dbShifts)
            {
                ShiftDto shift = new ShiftDto(s);
                shifts.Add(shift);
            }
            return Ok(shifts);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    #region Backup / Restore
    [HttpGet("Backup")]
    public async Task<IActionResult> Backup()
    {
        string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";
        try
        {
            System.IO.File.Delete(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        bool success = await BackupDatabaseAsync(path);
        if (success)
        {
            return Ok("Backup was successful");
        }
        else
        {
            return BadRequest("Backup failed");
        }
    }

    [HttpGet("Restore")]
    public async Task<IActionResult> Restore()
    {
        string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";

        bool success = await RestoreDatabaseAsync(path);
        if (success)
        {
            return Ok("Restore was successful");
        }
        else
        {
            return BadRequest("Restore failed");
        }
    }
    //this function backup the database to a specified path
    private async Task<bool> BackupDatabaseAsync(string path)
    {
        try
        {

            //Get the connection string
            string? connectionString = context.Database.GetConnectionString();
            //Get the database name
            string databaseName = context.Database.GetDbConnection().Database;
            //Build the backup command
            string command = $"BACKUP DATABASE {databaseName} TO DISK = '{path}'";
            //Create a connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Open the connection
                await connection.OpenAsync();
                //Create a command
                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                {
                    //Execute the command
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    //THis function restore the database from a backup in a certain path
    private async Task<bool> RestoreDatabaseAsync(string path)
    {
        try
        {
            //Get the connection string
            string? connectionString = context.Database.GetConnectionString();
            //Get the database name
            string databaseName = context.Database.GetDbConnection().Database;
            //Build the restore command
            string command = $@"
               USE master;
               DECLARE @latestBackupSet INT;
               SELECT TOP 1 @latestBackupSet = position
               FROM msdb.dbo.backupset
               WHERE database_name = '{databaseName}'
               AND backup_set_id IN (
                     SELECT backup_set_id
                     FROM msdb.dbo.backupmediafamily
                     WHERE physical_device_name = '{path}'
                 )
               ORDER BY backup_start_date DESC;
                ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE {databaseName} FROM DISK = '{path}' 
                WITH FILE=@latestBackupSet,
                REPLACE;
                ALTER DATABASE {databaseName} SET MULTI_USER;";

            //Create a connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Open the connection
                await connection.OpenAsync();
                //Create a command
                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                {
                    //Execute the command
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }
    #endregion

}












