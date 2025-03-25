using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("WorkerShiftRequest")]
public partial class WorkerShiftRequest
{
    [Key]
    [Column("RequestID")]
    public int RequestId { get; set; }

    public int WeekNum { get; set; }

    public int Year { get; set; }

    public int WorkerId { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; } = null!;

    [ForeignKey("WorkerId")]
    [InverseProperty("WorkerShiftRequests")]
    public virtual Worker Worker { get; set; } = null!;
}
