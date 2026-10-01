using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class GioHang
    {
        [Key]
        public int MaGH { get; set; }

        public int MaKH { get; set; }

        public int MaSP { get; set; }

        public int SoLuong { get; set; }

        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
    }
}