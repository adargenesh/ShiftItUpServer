using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShiftItUpServer.Models;

public partial class ShiftItUpDbContext : DbContext
{
    public ShiftItUpDbContext()
    {
    }

    public ShiftItUpDbContext(DbContextOptions<ShiftItUpDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DefiningShift> DefiningShifts { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    public virtual DbSet<WorkerInShift> WorkerInShifts { get; set; }

    public virtual DbSet<WorkerShiftRequest> WorkerShiftRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=ShiftItUpDB;User ID=ShiftAdminLogin;Password=kukuPassword;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DefiningShift>(entity =>
        {
            entity.HasKey(e => e.DefiningShiftId).HasName("PK__Defining__CC609F429747D210");

            entity.HasOne(d => d.IdStoreNavigation).WithMany(p => p.DefiningShifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DefiningS__IdSto__35BCFE0A");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__Shift__C0A838E1E949D205");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC075C72630B");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.IdStore).HasName("PK__Store__2A8EB27861F64E62");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.WorkerId).HasName("PK__Worker__077C8826D77EFA08");

            entity.HasOne(d => d.IdStoreNavigation).WithMany(p => p.Workers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Worker__IdStore__2A4B4B5E");

            entity.HasOne(d => d.StatusWorkerNavigation).WithMany(p => p.Workers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Worker__StatusWo__2B3F6F97");
        });

        modelBuilder.Entity<WorkerInShift>(entity =>
        {
            entity.HasOne(d => d.Shift).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkerInS__Shift__2F10007B");

            entity.HasOne(d => d.Worker).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkerInS__Worke__300424B4");
        });

        modelBuilder.Entity<WorkerShiftRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__WorkerSh__33A8519ADECC7E45");

            entity.HasOne(d => d.Worker).WithMany(p => p.WorkerShiftRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkerShi__Worke__32E0915F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
