using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Feedback
{
    public string FeedbackId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? CourseId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public bool? HiddenStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Account? User { get; set; }
}
