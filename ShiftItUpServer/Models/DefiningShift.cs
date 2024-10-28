using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("DefiningShift")]
public partial class DefiningShift
{
    [Key]
    [Column("ShiftID")]
    public int ShiftId { get; set; }

    public int IdStore { get; set; }

    public DateOnly ShiftDate { get; set; }

    [StringLength(50)]
    public string ShiftHour { get; set; } = null!;

    [ForeignKey("IdStore")]
    [InverseProperty("DefiningShifts")]
    public virtual Store IdStoreNavigation { get; set; } = null!;
}
