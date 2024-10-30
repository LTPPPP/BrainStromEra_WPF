using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? CourseDescription { get; set; }

    public int? CourseStatus { get; set; }

    public string? CoursePicture { get; set; }

    public decimal Price { get; set; }

    public DateTime CourseCreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual Status? CourseStatusNavigation { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();
}
