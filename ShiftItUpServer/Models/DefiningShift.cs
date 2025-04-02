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
    [Column("DefiningShiftID")]
    public int DefiningShiftId { get; set; }

    public int IdStore { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int NumEmployees { get; set; }

    [ForeignKey("IdStore")]
    [InverseProperty("DefiningShifts")]
    public virtual Store IdStoreNavigation { get; set; } = null!;
}
