using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class NhanVien
    {
        [Key]
        public int MaNV { get; set; }

        public string TenNV { get; set; } = "";

        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$",
    ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
        public string SoDienThoai { get; set; } = "";

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public int MaQuyen { get; set; }

        public bool TrangThai { get; set; }

        [ForeignKey("MaQuyen")]
        public PhanQuyen? PhanQuyen { get; set; }
    }
}