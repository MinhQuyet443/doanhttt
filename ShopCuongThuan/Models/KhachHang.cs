using System.ComponentModel.DataAnnotations;

namespace ShopCuongThuan.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        public string HoTen { get; set; } = "";

        public string Email { get; set; } = "";

        public string? SoDienThoai { get; set; }

        public string? DiaChi { get; set; }

        public string MatKhau { get; set; } = "";

        public DateTime NgayTao { get; set; }

        public List<GioHang> GioHangs { get; set; } = new();

        public List<DonHang> DonHangs { get; set; } = new();
    }
}