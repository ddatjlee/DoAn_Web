using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class PhieuXuat
{
    public int MaPx { get; set; }

    public DateOnly NgayLap { get; set; }

    public string MaNv { get; set; } = null!;

    public virtual ICollection<Ctpx> Ctpxes { get; set; } = new List<Ctpx>();

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
