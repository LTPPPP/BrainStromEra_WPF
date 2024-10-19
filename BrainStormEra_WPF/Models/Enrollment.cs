using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Enrollment
{
    public string EnrollmentId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? CourseId { get; set; }

    public int? EnrollmentStatus { get; set; }

    public DateOnly? CertificateIssuedDate { get; set; }

    public DateTime EnrollmentCreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Status? EnrollmentStatusNavigation { get; set; }

    public virtual Account? User { get; set; }
}
