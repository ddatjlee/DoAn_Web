using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class SanPham
{
    public string MaSp { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public string MaLoai { get; set; } = null!;

    public virtual ICollection<Ctpx> Ctpxes { get; set; } = new List<Ctpx>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;
}
