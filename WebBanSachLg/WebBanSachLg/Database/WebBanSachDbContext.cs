using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBanSachLg.Database;

public partial class WebBanSachDbContext : DbContext
{
    public WebBanSachDbContext()
    {
    }

    public WebBanSachDbContext(DbContextOptions<WebBanSachDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<TacGium> TacGia { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3214EC0793C52D1E");

            entity.ToTable("ChiTietDonHang");

            entity.HasIndex(e => e.DonHangId, "IX_ChiTietDonHang_DonHangId");

            entity.HasIndex(e => e.SachId, "IX_ChiTietDonHang_SachId");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.DonHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK_ChiTietDonHang_DonHang");

            entity.HasOne(d => d.Sach).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.SachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_Sach");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietG__3214EC07078FDE42");

            entity.ToTable("ChiTietGioHang");

            entity.HasIndex(e => e.GioHangId, "IX_ChiTietGioHang_GioHangId");

            entity.HasIndex(e => e.SachId, "IX_ChiTietGioHang_SachId");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.GioHang).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.GioHangId)
                .HasConstraintName("FK_ChiTietGioHang_GioHang");

            entity.HasOne(d => d.Sach).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.SachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietGioHang_Sach");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMuc__3214EC072C1E0891");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonHang__3214EC0777CA58E1");

            entity.ToTable("DonHang");

            entity.HasIndex(e => e.NgayDat, "IX_DonHang_NgayDat");

            entity.HasIndex(e => e.TaiKhoanId, "IX_DonHang_TaiKhoanId");

            entity.HasIndex(e => e.TrangThai, "IX_DonHang_TrangThai");

            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(500);
            entity.Property(e => e.HoTenNguoiNhan).HasMaxLength(255);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValueSql("(N'Chờ xử lý')");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_TaiKhoan");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GioHang__3214EC07AE922B4F");

            entity.ToTable("GioHang");

            entity.HasIndex(e => e.TaiKhoanId, "IX_GioHang_TaiKhoanId");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_TaiKhoan");
        });

        modelBuilder.Entity<NhaXuatBan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhaXuatB__3214EC0740EF0DA7");

            entity.ToTable("NhaXuatBan");

            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenNhaXuatBan).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sach__3214EC073124A565");

            entity.ToTable("Sach");

            entity.HasIndex(e => e.DanhMucId, "IX_Sach_DanhMucId");

            entity.HasIndex(e => e.NhaXuatBanId, "IX_Sach_NhaXuatBanId");

            entity.HasIndex(e => e.TacGiaId, "IX_Sach_TacGiaId");

            entity.HasIndex(e => e.TenSach, "IX_Sach_TenSach");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenSach).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.Saches)
                .HasForeignKey(d => d.DanhMucId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sach_DanhMuc");

            entity.HasOne(d => d.NhaXuatBan).WithMany(p => p.Saches)
                .HasForeignKey(d => d.NhaXuatBanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sach_NhaXuatBan");

            entity.HasOne(d => d.TacGia).WithMany(p => p.Saches)
                .HasForeignKey(d => d.TacGiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sach_TacGia");
        });

        modelBuilder.Entity<TacGium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TacGia__3214EC07F3BEE357");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenTacGia).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaiKhoan__3214EC070C7778FF");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "IX_TaiKhoan_Email");

            entity.HasIndex(e => e.TenDangNhap, "IX_TaiKhoan_TenDangNhap");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0D18F10B4").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenDangNhap).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasDefaultValueSql("('User')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
