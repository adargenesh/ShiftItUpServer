using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

[Table("Store")]
[Index("ManagerEmail", Name = "UQ__Store__351A32D7692E4A54", IsUnique = true)]
public partial class Store
{
    [Key]
    public int IdStore { get; set; }

    [StringLength(50)]
    public string StoreName { get; set; } = null!;

    [StringLength(50)]
    public string StoreAdress { get; set; } = null!;

    [StringLength(50)]
    public string StoreManager { get; set; } = null!;

    [StringLength(50)]
    public string ManagerEmail { get; set; } = null!;

    [StringLength(50)]
    public string ManagerPassword { get; set; } = null!;

    [InverseProperty("IdStoreNavigation")]
    public virtual ICollection<DefiningShift> DefiningShifts { get; set; } = new List<DefiningShift>();

    [InverseProperty("IdStoreNavigation")]
    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
