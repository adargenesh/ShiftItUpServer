using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftItUpServer.DTO;
using ShiftItUpServer.Models;

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
            if (modelsUser == null || modelsUser.UserPassword != loginDto.UserPassword)
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

    [HttpPost("registerStore")]
    public IActionResult RegisterStore([FromBody] ShiftItUpServer.DTO.StoreDto storeDto)
    {
        try
        {
            HttpContext.Session.Clear(); //Logout any previous login attempt

            //Create model store class
            ShiftItUpServer.Models.Store modelsStore = storeDto.GetModel();

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

    private string? GetLoggedInEmail()
    {
        string? email = HttpContext.Session.GetString("loggedInUser");
        if (string.IsNullOrEmpty(email))
        {
            email = HttpContext.Session.GetString("loggedInStore");
        }
        return email;

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
        user.UserStoreName = userDto.UserStoreName; 
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
















}












