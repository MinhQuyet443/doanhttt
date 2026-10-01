using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class ChiTietPhieuNhap
    {
        [Key]
        public int MaCTPN { get; set; }

        public int MaPN { get; set; }

        public int? MaSP { get; set; }

        public int SoLuongNhap { get; set; }

        public decimal DonGiaNhap { get; set; }

        public decimal ThanhTien { get; set; }

        [ForeignKey("MaPN")]
        public PhieuNhap? PhieuNhap { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
    }
}