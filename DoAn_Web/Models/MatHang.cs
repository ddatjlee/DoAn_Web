using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class MatHang
{
    public string MaMh { get; set; } = null!;

    public string TenMh { get; set; } = null!;

    public string? DonViTinh { get; set; }

    public decimal DonGiaMua { get; set; }

    public int? SoLuongTon { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
