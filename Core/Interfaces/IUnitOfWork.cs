namespace Core.Interfaces;

using Core.Entities.Students;
using Core.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork:IDisposable
{
    //IAuthorRepository Authors { get; } //property for Author repository
    //IBookRepository Books { get; } //property for Book repository
    int Complete();

    // Added methods to allow controllers to use UnitOfWork directly
    Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default);
    Task<Student?> GetStudentByIdAsync(Guid id, CancellationToken ct = default);
    Task<Student?> GetStudentWithEnrollmentsAsync(Guid id, CancellationToken ct = default);

    // Submit student answers: returns created result id or Guid.Empty if not implemented
    Task<Guid> SubmitStudentAnswersAsync(Guid examResultId, IEnumerable<(Guid QuestionId, Guid SelectedAnswerId)> answers, CancellationToken ct = default);

    // Enrollment related operations
    Task<Enrollment?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken ct = default);
    Task<Enrollment?> CreateEnrollmentAsync(Guid studentId, Guid courseId, DateTimeOffset enrollmentDate, CancellationToken ct = default);
    Task<Enrollment?> UpdateEnrollmentStatusAsync(Guid enrollmentId, string status, string? reason, CancellationToken ct = default);
    Task<bool> CancelEnrollmentAsync(Guid enrollmentId, string? reason, CancellationToken ct = default);

    // Commit (async)
    Task CommitAsync(CancellationToken ct = default);
}


