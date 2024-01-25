using System;
using System.Collections.Generic;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data;

public partial class QlksdbContext : DbContext
{
    public QlksdbContext() { }

    public QlksdbContext(DbContextOptions<QlksdbContext> options)
        : base(options) { }

    public virtual DbSet<DatDichVuModel>? DatDichVus { get; set; }

    public virtual DbSet<DatPhongModel>? DatPhongs { get; set; }

    public virtual DbSet<DichVuModel>? DichVus { get; set; }

    public virtual DbSet<HoaDonDatPhongModel>? HoaDonDatPhongs { get; set; }

    public virtual DbSet<KhachHangModel>? KhachHangs { get; set; }

    public virtual DbSet<LoaiPhongModel>? LoaiPhongs { get; set; }

    public virtual DbSet<LuongNhanVienModel>? LuongNhanViens { get; set; }

    public virtual DbSet<NhanVienModel>? NhanViens { get; set; }

    public virtual DbSet<PhongModel>? Phongs { get; set; }

    public virtual DbSet<RoleModel>? Roles { get; set; }

    public virtual DbSet<TacVuPhongModel>? TacVuPhongs { get; set; }

    public virtual DbSet<TaiKhoanModel>? TaiKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer("Name=ConnectionStrings:strCon");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatDichVuModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DatDichV__3213E83F26293BF6");

            entity.Property(e => e.MaDdv).IsFixedLength();
            entity.Property(e => e.MaDp).IsFixedLength();
            entity.Property(e => e.MaDv).IsFixedLength();
            entity.Property(e => e.NgayDatDichVu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuong).HasDefaultValueSql("((1))");

            entity
                .HasOne(d => d.MaDpNavigation)
                .WithMany(p => p.DatDichVus)
                .HasPrincipalKey(p => p.MaDp)
                .HasForeignKey(d => d.MaDp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatDichVu607749");

            entity
                .HasOne(d => d.MaDvNavigation)
                .WithMany(p => p.DatDichVus)
                .HasPrincipalKey(p => p.MaDv)
                .HasForeignKey(d => d.MaDv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatDichVu404793");
        });

        modelBuilder.Entity<DatPhongModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DatPhong__3213E83FF42E97D5");

            entity.Property(e => e.MaDp).IsFixedLength();
            entity.Property(e => e.MaKh).IsFixedLength();
            entity.Property(e => e.MaP).IsFixedLength();
            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(getdate())");

            entity
                .HasOne(d => d.MaKhNavigation)
                .WithMany(p => p.DatPhongs)
                .HasPrincipalKey(p => p.MaKh)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatPhong46426");

            entity
                .HasOne(d => d.MaPNavigation)
                .WithMany(p => p.DatPhongs)
                .HasPrincipalKey(p => p.MaP)
                .HasForeignKey(d => d.MaP)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDatPhong908988");
        });

        modelBuilder.Entity<DichVuModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DichVu__3213E83F0D62EFC6");

            entity.Property(e => e.MaDv).IsFixedLength();
        });

        modelBuilder.Entity<HoaDonDatPhongModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDonDa__3213E83FC0182997");

            entity.Property(e => e.MaDp).IsFixedLength();
            entity.Property(e => e.MaHddp).IsFixedLength();
            entity.Property(e => e.MaNv).IsFixedLength();
            entity.Property(e => e.NgayHd).HasDefaultValueSql("(getdate())");

            entity
                .HasOne(d => d.MaDpNavigation)
                .WithOne(p => p.HoaDonDatPhong)
                .HasPrincipalKey<DatPhongModel>(p => p.MaDp)
                .HasForeignKey<HoaDonDatPhongModel>(d => d.MaDp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKHoaDonDatP734856");

            entity
                .HasOne(d => d.MaNvNavigation)
                .WithMany(p => p.HoaDonDatPhongs)
                .HasPrincipalKey(p => p.MaNv)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKHoaDonDatP269546");
        });

        modelBuilder.Entity<KhachHangModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhachHan__3213E83F8FFAE1AB");

            entity.Property(e => e.MaKh).IsFixedLength();
        });

        modelBuilder.Entity<LoaiPhongModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoaiPhon__3213E83FF094A36E");

            entity.Property(e => e.DonGia).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<LuongNhanVienModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LuongNha__3213E83FF8A09411");

            entity.Property(e => e.MaNv).IsFixedLength();

            entity
                .HasOne(d => d.MaNvNavigation)
                .WithMany(p => p.LuongNhanViens)
                .HasPrincipalKey(p => p.MaNv)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKLuongNhanV939854");
        });

        modelBuilder.Entity<NhanVienModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhanVien__3213E83F1F3AC476");

            entity.Property(e => e.MaNv).IsFixedLength();

            entity
                .HasOne(d => d.UserNameNavigation)
                .WithMany(p => p.NhanViens)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.UserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNhanVien442527");
        });

        modelBuilder.Entity<PhongModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Phong__3213E83F7772FD22");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaP).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity
                .HasOne(d => d.MaLpNavigation)
                .WithMany(p => p.Phongs)
                .HasPrincipalKey(p => p.MaLp)
                .HasForeignKey(d => d.MaLp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPhong696843");

            entity
                .HasOne(d => d.MaTvpNavigation)
                .WithMany(p => p.Phongs)
                .HasPrincipalKey(p => p.MaTvp)
                .HasForeignKey(d => d.MaTvp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPhong845765");
        });

        modelBuilder.Entity<RoleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F499C7EFD");
        });

        modelBuilder.Entity<TacVuPhongModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TacVuPho__3213E83FAED69608");
        });

        modelBuilder.Entity<TaiKhoanModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaiKhoan__3213E83F28895CA4");

            entity.Property(e => e.CoutLoginFailed).HasDefaultValueSql("((0))");

            entity
                .HasOne(d => d.IdRoleNavigation)
                .WithMany(p => p.TaiKhoans)
                .HasPrincipalKey(p => p.IdRole)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTaiKhoan365359");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
