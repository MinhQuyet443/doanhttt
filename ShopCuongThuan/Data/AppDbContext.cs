using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMuc> DanhMuc { get; set; }

        public DbSet<SanPham> SanPham { get; set; }

        public DbSet<KhuyenMai> KhuyenMai { get; set; }

        public DbSet<KhachHang> KhachHang { get; set; }

        public DbSet<GioHang> GioHang { get; set; }

        public DbSet<DonHang> DonHang { get; set; }

        public DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }

        public DbSet<NhanVien> NhanVien { get; set; }

        public DbSet<PhanQuyen> PhanQuyen { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DanhMuc>().ToTable("DanhMuc");
            modelBuilder.Entity<SanPham>().ToTable("SanPham");
            modelBuilder.Entity<KhuyenMai>().ToTable("KhuyenMai");
            modelBuilder.Entity<KhachHang>().ToTable("KhachHang");
            modelBuilder.Entity<GioHang>().ToTable("GioHang");
            modelBuilder.Entity<DonHang>().ToTable("DonHang");
            modelBuilder.Entity<ChiTietDonHang>().ToTable("ChiTietDonHang");
            modelBuilder.Entity<NhanVien>().ToTable("NhanVien");
            modelBuilder.Entity<PhanQuyen>().ToTable("PhanQuyen");
            modelBuilder.Entity<PhieuNhap>().ToTable("PhieuNhap");
            modelBuilder.Entity<ChiTietPhieuNhap>().ToTable("ChiTietPhieuNhap");
            
   
        }
    }
}