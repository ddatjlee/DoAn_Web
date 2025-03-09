using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Web.Models;

public partial class MasterContext : DbContext
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<Ctpx> Ctpxes { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<MatHang> MatHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuXuat> PhieuXuats { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-F2I8231\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.SoHd, e.MaMh }).HasName("PK__Chi_Tiet__3E4EF6AAE6696C06");

            entity.ToTable("Chi_Tiet_Hoa_Don");

            entity.Property(e => e.SoHd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SoHD");
            entity.Property(e => e.MaMh)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KhuyenMai).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chi_Tiet_H__MaMH__2D47B39A");

            entity.HasOne(d => d.SoHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.SoHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chi_Tiet_H__SoHD__2C538F61");
        });

        modelBuilder.Entity<Ctpx>(entity =>
        {
            entity.HasKey(e => new { e.MaPx, e.MaSp });

            entity.ToTable("CTPX", tb => tb.HasTrigger("T1"));

            entity.Property(e => e.MaPx).HasColumnName("MaPX");
            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");

            entity.HasOne(d => d.MaPxNavigation).WithMany(p => p.Ctpxes)
                .HasForeignKey(d => d.MaPx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPX_PhieuXuat");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.Ctpxes)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPX_SanPham");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.SoHd).HasName("PK__Hoa_Don__BC3CAB5724AE050B");

            entity.ToTable("Hoa_Don");

            entity.Property(e => e.SoHd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SoHD");
            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.TongTriGia).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Hoa_Don__MaKH__297722B6");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__Khach_Ha__2725CF1E6D7360B6");

            entity.ToTable("Khach_Hang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenKh)
                .HasMaxLength(50)
                .HasColumnName("TenKH");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__Loai__730A575928FC746B");

            entity.ToTable("Loai");

            entity.Property(e => e.MaLoai)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<MatHang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__Mat_Hang__2725DFD99B75EFE0");

            entity.ToTable("Mat_Hang", tb => tb.HasTrigger("TenT1"));

            entity.Property(e => e.MaMh)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.DonGiaMua).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh).HasMaxLength(10);
            entity.Property(e => e.TenMh)
                .HasMaxLength(50)
                .HasColumnName("TenMH");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70AAA5B1B9A");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Phai).HasDefaultValue(false);
        });

        modelBuilder.Entity<PhieuXuat>(entity =>
        {
            entity.HasKey(e => e.MaPx).HasName("PK__PhieuXua__2725E7CAE9B18E10");

            entity.ToTable("PhieuXuat");

            entity.Property(e => e.MaPx)
                .ValueGeneratedNever()
                .HasColumnName("MaPX");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuXuats)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuXuat_NhanVien");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081C6A564902");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.TenSp, "UQ__SanPham__4CF9DC14B44D606B").IsUnique();

            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.MaLoai)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenSp)
                .HasMaxLength(50)
                .HasColumnName("TenSP");

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_Loai");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
