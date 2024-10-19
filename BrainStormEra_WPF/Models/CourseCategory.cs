using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class CourseCategory
{
    public string CourseCategoryId { get; set; } = null!;

    public string CourseCategoryName { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
