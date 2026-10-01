namespace ShopCuongThuan.Models
{
    public class SanPhamCardViewModel
    {
        public int MaSP { get; set; }

        public string TenSP { get; set; } = "";

        public string? Hang { get; set; }

        public string? TenDM { get; set; }

        public decimal GiaBan { get; set; }

        public decimal GiaHienThi { get; set; }

        public bool CoKhuyenMai { get; set; }

        public string? GiamGiaText { get; set; }

        public string? TenKhuyenMai { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public int SoLuongTon { get; set; }
    }
}