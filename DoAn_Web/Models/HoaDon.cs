using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class HoaDon
{
    public string SoHd { get; set; } = null!;

    public DateOnly NgayHoaDon { get; set; }

    public string MaKh { get; set; } = null!;

    public decimal TongTriGia { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
