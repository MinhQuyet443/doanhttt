using System.ComponentModel.DataAnnotations;

namespace ShopCuongThuan.Models
{
    public class PhanQuyen
    {
        [Key]
        public int MaQuyen { get; set; }

        public string TenQuyen { get; set; } = "";

        public List<NhanVien> NhanViens { get; set; } = new();
    }
}