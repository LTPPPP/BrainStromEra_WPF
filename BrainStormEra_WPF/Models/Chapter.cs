using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Chapter
{
    public string ChapterId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string ChapterName { get; set; } = null!;

    public string? ChapterDescription { get; set; }

    public int? ChapterOrder { get; set; }

    public int? ChapterStatus { get; set; }

    public DateTime ChapterCreatedAt { get; set; }

    public virtual Status? ChapterStatusNavigation { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
