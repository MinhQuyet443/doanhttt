namespace ShopCuongThuan.Models
{
    public class KiemKeKhoViewModel
    {
        public int MaSP { get; set; }

        public string TenSP { get; set; } = "";

        public string? HinhAnh { get; set; }

        public string? DanhMuc { get; set; }

        public string? Hang { get; set; }

        public decimal GiaBan { get; set; }

        public int TonHeThong { get; set; }

        public int TonThucTe { get; set; }

        public int ChenhLech
        {
            get
            {
                return TonThucTe - TonHeThong;
            }
        }

        public decimal GiaTriChenhLech
        {
            get
            {
                return ChenhLech * GiaBan;
            }
        }
    }
}