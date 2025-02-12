using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("Shift")]
public partial class Shift
{
    [Key]
    [Column("ShiftID")]
    public int ShiftId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ShiftStart { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ShiftEnd { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal SalesGoal { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? SalesActual { get; set; }
}
