using System.ComponentModel.DataAnnotations;

namespace ShopCuongThuan.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        public string HoTen { get; set; } = "";

        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$",
    ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
        public string SoDienThoai { get; set; } = "";

        public string? DiaChi { get; set; }

        public string MatKhau { get; set; } = "";

        public DateTime NgayTao { get; set; }

        public List<GioHang> GioHangs { get; set; } = new();

        public List<DonHang> DonHangs { get; set; } = new();
    }
}