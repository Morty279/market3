using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace market3.DataBase;

public partial class InternetMarketBalkaContext : DbContext
{
    public InternetMarketBalkaContext()
    {
    }

    public InternetMarketBalkaContext(DbContextOptions<InternetMarketBalkaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<TovarInZakaz> TovarInZakazs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zakaz> Zakazs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=192.168.200.13;database=\"internet-market Balka\";user=student;password=student;treattinyasboolean=true", ServerVersion.Parse("10.3.39-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tovar");

            entity.HasIndex(e => e.CategoryId, "FK_tovar_category_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("Category id");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasColumnType("blob");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(19, 2);
            entity.Property(e => e.Quantity).HasColumnType("int(11)");

            entity.HasOne(d => d.Category).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tovar_category_Id");
        });

        modelBuilder.Entity<TovarInZakaz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tovar _in_ zakaz");

            entity.HasIndex(e => e.TovarId, "FK_tovar _in_ zakaz_tovar_Id");

            entity.HasIndex(e => e.ZakazId, "FK_tovar _in_ zakaz_zakaz_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Price).HasPrecision(19, 2);
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
            entity.Property(e => e.TovarId)
                .HasColumnType("int(11)")
                .HasColumnName("Tovar_id");
            entity.Property(e => e.ZakazId)
                .HasColumnType("int(11)")
                .HasColumnName("Zakaz_id");

            entity.HasOne(d => d.Tovar).WithMany(p => p.TovarInZakazs)
                .HasForeignKey(d => d.TovarId)
                .HasConstraintName("FK_tovar _in_ zakaz_tovar_Id");

            entity.HasOne(d => d.Zakaz).WithMany(p => p.TovarInZakazs)
                .HasForeignKey(d => d.ZakazId)
                .HasConstraintName("FK_tovar _in_ zakaz_zakaz_Id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.RoleId, "FK_user_role_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId)
                .HasColumnType("int(11)")
                .HasColumnName("Role id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_role_Id");
        });

        modelBuilder.Entity<Zakaz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("zakaz");

            entity.HasIndex(e => e.UserId, "FK_zakaz_user_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.OrderDate)
                .HasColumnType("time")
                .HasColumnName("Order_date");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10)
                .HasColumnName("Total_price");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("User_id");

            entity.HasOne(d => d.User).WithMany(p => p.Zakazs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_zakaz_user_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
