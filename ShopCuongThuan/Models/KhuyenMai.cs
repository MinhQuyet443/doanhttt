using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class KhuyenMai
    {
        [Key]
        public int MaKM { get; set; }

        public int MaSP { get; set; }

        public string TenKM { get; set; } = "";

        public string LoaiGiamGia { get; set; } = "";

        public decimal GiaTriGiam { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        public bool TrangThai { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
    }
}