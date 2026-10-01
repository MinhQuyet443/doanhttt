using System.ComponentModel.DataAnnotations;

namespace ShopCuongThuan.Models
{
    public class DanhMuc
    {
        [Key]
        public int MaDM { get; set; }

        public string TenDM { get; set; } = "";

        public List<SanPham> SanPhams { get; set; } = new();
    }
}