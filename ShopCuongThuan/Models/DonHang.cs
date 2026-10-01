using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class DonHang
    {
        [Key]
        public int MaDH { get; set; }

        public int MaKH { get; set; }

        public string MaTraCuu { get; set; } = "";

        public DateTime NgayDat { get; set; }

        public string HoTenNhan { get; set; } = "";

        public string SoDienThoai { get; set; } = "";

        public string DiaChi { get; set; } = "";

        public decimal TongTien { get; set; }

        public string TrangThai { get; set; } = "";
        public string? PhuongThucThanhToan { get; set; }

        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }

        public List<ChiTietDonHang> ChiTietDonHangs { get; set; } = new();
    }
}