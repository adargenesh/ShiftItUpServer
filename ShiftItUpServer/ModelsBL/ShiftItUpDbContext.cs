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

    public Store? GetStore(string email)
    {
        return this.Stores.Include(s=> s.DefiningShifts).Where(u => u.ManagerEmail == email)
                            .FirstOrDefault();
    }

    public List<Worker>? GetAllWorkers()
    {
        return this.Workers.ToList();
    }
}

