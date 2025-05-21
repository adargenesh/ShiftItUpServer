using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("Worker")]
[Index("UserEmail", Name = "UQ__Worker__08638DF8F003D5F6", IsUnique = true)]
public partial class Worker
{
    [Key]
    public int WorkerId { get; set; }

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    public string UserLastName { get; set; } = null!;

    [StringLength(50)]
    public string UserEmail { get; set; } = null!;

    [StringLength(50)]
    public string UserPassword { get; set; } = null!;

    public int IdStore { get; set; }

    [StringLength(50)]
    public string UserSalary { get; set; } = null!;

    public int StatusWorker { get; set; }

    [ForeignKey("IdStore")]
    [InverseProperty("Workers")]
    public virtual Store IdStoreNavigation { get; set; } = null!;

    [ForeignKey("StatusWorker")]
    [InverseProperty("Workers")]
    public virtual Status StatusWorkerNavigation { get; set; } = null!;

    [InverseProperty("Worker")]
    public virtual ICollection<WorkerInShift> WorkerInShifts { get; set; } = new List<WorkerInShift>();

    [InverseProperty("Worker")]
    public virtual ICollection<WorkerShiftRequest> WorkerShiftRequests { get; set; } = new List<WorkerShiftRequest>();
}
