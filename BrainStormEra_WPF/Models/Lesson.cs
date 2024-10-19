using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Lesson
{
    public string LessonId { get; set; } = null!;

    public string? ChapterId { get; set; }

    public string LessonName { get; set; } = null!;

    public string? LessonDescription { get; set; }

    public string LessonContent { get; set; } = null!;

    public int LessonOrder { get; set; }

    public int? LessonTypeId { get; set; }

    public int? LessonStatus { get; set; }

    public DateTime LessonCreatedAt { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Status? LessonStatusNavigation { get; set; }

    public virtual LessonType? LessonType { get; set; }
}
