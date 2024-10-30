using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BrainStormEra_WPF.Models;

public partial class PrnDbFpContext : DbContext
{
    public PrnDbFpContext()
    {
    }

    public PrnDbFpContext(DbContextOptions<PrnDbFpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server= LTP;Database=PRN_DB_FP;uid=sa;pwd=01654460072ltp;encrypt=true;trustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__account__B9BE370F1CE0719D");

            entity.ToTable("account");

            entity.HasIndex(e => e.UserEmail, "UQ__account__B0FBA21240273EFC").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__account__F3DBC572C4D97DFB").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");
            entity.Property(e => e.AccountCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("account_created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.UserAddress)
                .HasColumnType("text")
                .HasColumnName("user_address");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserPicture).HasColumnName("User_picture");
            entity.Property(e => e.UserRole).HasColumnName("user_role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__account__user_ro__440B1D61");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__chapter__745EFE87063F8047");

            entity.ToTable("chapter");

            entity.HasIndex(e => new { e.CourseId, e.ChapterOrder }, "unique_chapter_order_per_course").IsUnique();

            entity.Property(e => e.ChapterId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("chapter_id");
            entity.Property(e => e.ChapterCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("chapter_created_at");
            entity.Property(e => e.ChapterDescription).HasColumnName("chapter_description");
            entity.Property(e => e.ChapterName)
                .HasMaxLength(255)
                .HasColumnName("chapter_name");
            entity.Property(e => e.ChapterOrder).HasColumnName("chapter_order");
            entity.Property(e => e.ChapterStatus).HasColumnName("chapter_status");
            entity.Property(e => e.CourseId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_id");

            entity.HasOne(d => d.ChapterStatusNavigation).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.ChapterStatus)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__chapter__chapter__60A75C0F");

            entity.HasOne(d => d.Course).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__chapter__course___5FB337D6");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__course__8F1EF7AE4AE00CF2");

            entity.ToTable("course");

            entity.Property(e => e.CourseId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_id");
            entity.Property(e => e.CourseCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("course_created_at");
            entity.Property(e => e.CourseDescription).HasColumnName("course_description");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("course_name");
            entity.Property(e => e.CoursePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_picture");
            entity.Property(e => e.CourseStatus).HasColumnName("course_status");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.CourseStatusNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CourseStatus)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__course__course_s__59FA5E80");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__course__created___5AEE82B9");

            entity.HasMany(d => d.CourseCategories).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCategoryMapping",
                    r => r.HasOne<CourseCategory>().WithMany()
                        .HasForeignKey("CourseCategoryId")
                        .HasConstraintName("FK__course_ca__cours__66603565"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK__course_ca__cours__656C112C"),
                    j =>
                    {
                        j.HasKey("CourseId", "CourseCategoryId").HasName("PK__course_c__10F922204E694AC2");
                        j.ToTable("course_category_mapping");
                        j.IndexerProperty<string>("CourseId")
                            .HasMaxLength(255)
                            .IsUnicode(false)
                            .HasColumnName("course_id");
                        j.IndexerProperty<string>("CourseCategoryId")
                            .HasMaxLength(255)
                            .IsUnicode(false)
                            .HasColumnName("course_category_id");
                    });
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.CourseCategoryId).HasName("PK__course_c__FE7D58E872173548");

            entity.ToTable("course_category");

            entity.Property(e => e.CourseCategoryId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_category_id");
            entity.Property(e => e.CourseCategoryName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_category_name");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__enrollme__6D24AA7AC7A4ABD7");

            entity.ToTable("enrollment");

            entity.Property(e => e.EnrollmentId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("enrollment_id");
            entity.Property(e => e.Approved)
                .HasDefaultValue(false)
                .HasColumnName("approved");
            entity.Property(e => e.CertificateIssuedDate).HasColumnName("certificate_issued_date");
            entity.Property(e => e.CourseId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_id");
            entity.Property(e => e.EnrollmentCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("enrollment_created_at");
            entity.Property(e => e.EnrollmentStatus).HasColumnName("enrollment_status");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__enrollmen__cours__72C60C4A");

            entity.HasOne(d => d.EnrollmentStatusNavigation).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.EnrollmentStatus)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__enrollmen__enrol__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__enrollmen__user___71D1E811");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__feedback__7A6B2B8C9164F6CF");

            entity.ToTable("feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("feedback_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CourseId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.HiddenStatus)
                .HasDefaultValue(false)
                .HasColumnName("hidden_status");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__feedback__course__7A672E12");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__feedback__user_i__797309D9");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__lesson__6421F7BE8FD461CC");

            entity.ToTable("lesson");

            entity.HasIndex(e => new { e.ChapterId, e.LessonOrder }, "unique_lesson_order_per_chapter").IsUnique();

            entity.Property(e => e.LessonId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lesson_id");
            entity.Property(e => e.ChapterId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("chapter_id");
            entity.Property(e => e.LessonContent).HasColumnName("lesson_content");
            entity.Property(e => e.LessonCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("lesson_created_at");
            entity.Property(e => e.LessonDescription).HasColumnName("lesson_description");
            entity.Property(e => e.LessonName)
                .HasMaxLength(255)
                .HasColumnName("lesson_name");
            entity.Property(e => e.LessonOrder).HasColumnName("lesson_order");
            entity.Property(e => e.LessonStatus).HasColumnName("lesson_status");
            entity.Property(e => e.LessonTypeId).HasColumnName("lesson_type_id");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__lesson__chapter___6B24EA82");

            entity.HasOne(d => d.LessonStatusNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonStatus)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__lesson__lesson_s__6D0D32F4");

            entity.HasOne(d => d.LessonType).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__lesson__lesson_t__6C190EBB");
        });

        modelBuilder.Entity<LessonType>(entity =>
        {
            entity.HasKey(e => e.LessonTypeId).HasName("PK__lesson_t__F5960D1E28C5F249");

            entity.ToTable("lesson_type");

            entity.Property(e => e.LessonTypeId)
                .ValueGeneratedNever()
                .HasColumnName("lesson_type_id");
            entity.Property(e => e.LessonTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lesson_type_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.UserRole).HasName("PK__role__68057FECD4C061E0");

            entity.ToTable("role");

            entity.Property(e => e.UserRole)
                .ValueGeneratedNever()
                .HasColumnName("user_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__status__3683B5314D827A19");

            entity.ToTable("status");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("status_description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
