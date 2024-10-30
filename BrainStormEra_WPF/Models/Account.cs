using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Account
{
    public string UserId { get; set; } = null!;

    public int? UserRole { get; set; }

    public string Username { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? UserAddress { get; set; }

    public byte[]? UserPicture { get; set; }

    public DateTime AccountCreatedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Role? UserRoleNavigation { get; set; }
}
