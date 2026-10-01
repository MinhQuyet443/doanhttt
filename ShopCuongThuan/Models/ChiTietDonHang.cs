using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaCTDH { get; set; }

        public int MaDH { get; set; }

        public int? MaSP { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        [ForeignKey("MaDH")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
    }
}