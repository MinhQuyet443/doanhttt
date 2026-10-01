using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class PhieuNhap
    {
        [Key]
        public int MaPN { get; set; }

        public string MaPhieuNhap { get; set; } = "";

        public DateTime NgayNhap { get; set; }

        public int? MaNV { get; set; }

        public decimal TongTien { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }

        public List<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new();
    }
}