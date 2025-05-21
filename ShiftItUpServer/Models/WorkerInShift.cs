using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("WorkerInShift")]
public partial class WorkerInShift
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ShiftID")]
    public int ShiftId { get; set; }

    public int WorkerId { get; set; }

    [StringLength(500)]
    public string? SystemRemarks { get; set; }

    [ForeignKey("ShiftId")]
    [InverseProperty("WorkerInShifts")]
    public virtual Shift Shift { get; set; } = null!;

    [ForeignKey("WorkerId")]
    [InverseProperty("WorkerInShifts")]
    public virtual Worker Worker { get; set; } = null!;
}
