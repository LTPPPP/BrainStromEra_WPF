using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
