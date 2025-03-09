using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;
}
