using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
