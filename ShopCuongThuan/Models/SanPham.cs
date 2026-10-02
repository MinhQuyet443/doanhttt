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

        [Range(0, double.MaxValue,
     ErrorMessage = "Giá sản phẩm không được âm")]
        public decimal GiaBan { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Số lượng không được âm")]
        public int SoLuongTon { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

    

        [ForeignKey("MaDM")]
        public DanhMuc? DanhMuc { get; set; }

        public List<KhuyenMai> KhuyenMais { get; set; } = new();

        [NotMapped]
        public IFormFile? UploadImage { get; set; }
    }
}