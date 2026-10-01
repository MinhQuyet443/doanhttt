using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class SanPham
    {
        [Key]
        public int MaSP { get; set; }

        public string TenSP { get; set; } = "";

        public int MaDM { get; set; }

        public string? Hang { get; set; }

        public decimal GiaBan { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public int SoLuongTon { get; set; }

        [ForeignKey("MaDM")]
        public DanhMuc? DanhMuc { get; set; }

        public List<KhuyenMai> KhuyenMais { get; set; } = new();

        [NotMapped]
        public IFormFile? UploadImage { get; set; }
    }
}