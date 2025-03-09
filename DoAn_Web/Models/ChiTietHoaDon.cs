using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class ChiTietHoaDon
{
    public string SoHd { get; set; } = null!;

    public string MaMh { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal? KhuyenMai { get; set; }

    public decimal GiaBan { get; set; }

    public virtual MatHang MaMhNavigation { get; set; } = null!;

    public virtual HoaDon SoHdNavigation { get; set; } = null!;
}
