using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

public partial class ShiftItUpDbContext : DbContext
{

    public Worker? GetUser(string email)
    {
        return this.Workers.Where(u => u.UserEmail == email)
                            .FirstOrDefault();
    }
}

