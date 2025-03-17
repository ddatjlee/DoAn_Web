using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class StudentSkill
{
    public int StudentID { get; set; }

    public int SkillID { get; set; }

    // Khai báo navigation properties
    public virtual Student Student { get; set; }
    public virtual Skill Skill { get; set; }
}