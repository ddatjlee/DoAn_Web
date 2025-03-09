using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class Ctpx
{
    public int MaPx { get; set; }

    public string MaSp { get; set; } = null!;

    public int SoLuong { get; set; }

    public virtual PhieuXuat MaPxNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
