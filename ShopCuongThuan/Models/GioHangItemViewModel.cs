namespace ShopCuongThuan.Models
{
    public class GioHangItemViewModel
    {
        public int MaSP { get; set; }

        public string TenSP { get; set; } = "";

        public string? HinhAnh { get; set; }

        public decimal GiaGoc { get; set; }

        public decimal DonGia { get; set; }

        public bool CoKhuyenMai { get; set; }

        public string? GiamGiaText { get; set; }

        public int SoLuong { get; set; }

        public decimal ThanhTien { get; set; }
    }
}