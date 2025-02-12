using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Keyless]
[Table("WorkerInShift")]
public partial class WorkerInShift
{
    [Column("ShiftID")]
    public int ShiftId { get; set; }

    public int WorkerId { get; set; }

    [StringLength(500)]
    public string? SystemRemarks { get; set; }

    [ForeignKey("ShiftId")]
    public virtual Shift Shift { get; set; } = null!;

    [ForeignKey("WorkerId")]
    public virtual Worker Worker { get; set; } = null!;
}
