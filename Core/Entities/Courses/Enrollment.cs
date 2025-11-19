using Core.Common;
using Core.Entities.Students;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Entities.Courses;

public partial class Enrollment : AuditableEntity
{
    public const string StatusActive = "Active";
    public const string StatusPending = "Pending";

    [ForeignKey("Student")]
    public Guid StudentId { get; private set; }


    [ForeignKey("Course")]
    public Guid CourseId { get; private set; }



    public DateTimeOffset EnrollmentDate { get; private set; }
    public string Status { get; private set; } = "Pending";

    public DateTimeOffset? StatusChangedAt { get; private set; }
    public string? StatusReason { get; private set; }

    // Navigation properties
    [JsonIgnore]
    public Student Student { get; private set; } = default!;
    [JsonIgnore]
    public Course Course { get; private set; } = default!;

    private Enrollment() { }

    private Enrollment(Guid studentId, Guid courseId, string status, DateTimeOffset? enrollmentDate = null)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        CourseId = courseId;
        Status = status;
        EnrollmentDate = enrollmentDate ?? DateTimeOffset.UtcNow;
        StatusChangedAt = DateTimeOffset.UtcNow;
    }

    public static Enrollment Create(Guid studentId, Guid courseId, string status)
    {
        return Create(studentId, courseId, status, null);
    }

    public static Enrollment Create(Guid studentId, Guid courseId, string status, DateTimeOffset? enrollmentDate = null)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("studentId must be provided", nameof(studentId));
        if (courseId == Guid.Empty)
            throw new ArgumentException("courseId must be provided", nameof(courseId));
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("status must be provided", nameof(status));

        return new Enrollment(studentId, courseId, status, enrollmentDate);
    }

    public void ChangeStatus(string status, string? reason)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("status must be provided", nameof(status));

        if (Status != status)
        {
            Status = status;
            StatusReason = reason;
            StatusChangedAt = DateTimeOffset.UtcNow;
        }
    }

    public object Cancel(string? cancellationReason)
    {
        throw new NotImplementedException();
    }

    public void SetStatus(string status, string? reason)
    {
        throw new NotImplementedException();
    }
}
