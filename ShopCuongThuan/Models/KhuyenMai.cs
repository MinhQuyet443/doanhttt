
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCuongThuan.Models
{
    public class KhuyenMai
    {
        [Key]
        public int MaKM { get; set; }

        public int MaSP { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khuyến mãi")]
        public string TenKM { get; set; } = "";

        public string LoaiGiamGia { get; set; } = "";

        [Range(0, double.MaxValue,
            ErrorMessage = "Giá trị giảm không được âm")]
        public decimal GiaTriGiam { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        public bool TrangThai { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
        [NotMapped]
        public string TrangThaiHienThi
        {
            get
            {
                if (!TrangThai)
                    return "Tạm dừng";

                var homNay = DateTime.Today;

                if (NgayBatDau.HasValue &&
                    NgayBatDau.Value.Date > homNay)
                    return "Sắp diễn ra";

                if (NgayKetThuc.HasValue &&
                    NgayKetThuc.Value.Date >= homNay)
                    return "Đang diễn ra";

                return "Tạm dừng";
            }
        }
    }
}