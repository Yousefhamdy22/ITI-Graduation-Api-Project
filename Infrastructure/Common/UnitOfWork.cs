using Infrastructure.Data;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Students;
using Infrastructure.Interface;
using Core.Entities.Exams;
using Core.Entities.Courses;

namespace Infrastructure.Common;

public class UnitOfWork: IUnitOfWork
{
    private readonly AppDBContext _context;
    private readonly IStudentRepository _studentRepo;
    private readonly IStudentAnswerRepository _studentAnswerRepo;
    private readonly IExamResultRepository _examResultRepo;
    private readonly IAnswerOptionRepository _answerOptionRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ICourseRepository _courseRepo;

    public UnitOfWork(AppDBContext context,
        IStudentRepository studentRepo,
        IStudentAnswerRepository studentAnswerRepo,
        IExamResultRepository examResultRepo,
        IAnswerOptionRepository answerOptionRepo,
        IEnrollmentRepository enrollmentRepo,
        ICourseRepository courseRepo)
    {
        _context = context;
        _studentRepo = studentRepo;
        _studentAnswerRepo = studentAnswerRepo;
        _examResultRepo = examResultRepo;
        _answerOptionRepo = answerOptionRepo;
        _enrollmentRepo = enrollmentRepo;
        _courseRepo = courseRepo;
    }

    //public IAuthorRepository Authors => new AuthorRepository(_context);
    //public IBookRepository Books => new BookRepository(_context);

    public int Complete()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default)
    {
        return await _studentRepo.GetAllAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Use FindAsync with predicate because generic GetById expects int key
        return await _studentRepo.FindAsync(s => s.Id == id);
    }

    public async Task<Student?> GetStudentWithEnrollmentsAsync(Guid id, CancellationToken ct = default)
    {
        return await _studentRepo.GetWithEnrollmentsAsync(id, ct);
    }

    public async Task<Guid> SubmitStudentAnswersAsync(Guid examResultId, IEnumerable<(Guid QuestionId, Guid SelectedAnswerId)> answers, CancellationToken ct = default)
    {
        // Basic flow: For each answer, create StudentAnswer entity and save. This assumes ExamResult exists.
        var examResult = await _examResultRepo.GetByIdWithDetailsAsync(examResultId, ct);
        if (examResult == null) return Guid.Empty;

        foreach (var a in answers)
        {
            var entity = StudentAnswer.Create(examResultId, a.QuestionId, a.SelectedAnswerId);
            await _studentAnswerRepo.AddAsync(entity, ct);
        }

        await _context.SaveChangesAsync(ct);

        return examResultId;
    }

    // Enrollment implementations
    public async Task<Enrollment?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdWithDetailsAsync(enrollmentId, ct);
        return enrollment; // repository returns Enrollment? directly
    }

    public async Task<Enrollment?> CreateEnrollmentAsync(Guid studentId, Guid courseId, DateTimeOffset enrollmentDate, CancellationToken ct = default)
    {
        // Validate student and course existence
        var student = await _studentRepo.FindAsync(s => s.Id == studentId);
        if (student == null) return null;

        // Use FindAsync for Guid key
        var course = await _courseRepo.FindAsync(c => c.Id == courseId);
        if (course == null) return null;

        // Check for existing enrollment
        var exists = await _enrollmentRepo.ExistsAsync(studentId, courseId, ct);
        if (exists) return null;

        // Determine status
        var status = course.TypeStatus == Course.TypeFree ? Enrollment.StatusActive : Enrollment.StatusPending;

        var enrollment = Core.Entities.Courses.Enrollment.Create(studentId, courseId, status);
        if (enrollment == null) return null;

        await _enrollmentRepo.AddAsync(enrollment, ct);
        await _context.SaveChangesAsync(ct);

        return enrollment;
    }

    public async Task<Enrollment?> UpdateEnrollmentStatusAsync(Guid enrollmentId, string status, string? reason, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdWithDetailsAsync(enrollmentId, ct);
        if (enrollment == null) return null;

        enrollment.ChangeStatus(status, reason);
        await _enrollmentRepo.UpdateAsync(enrollment, ct);
        await _context.SaveChangesAsync(ct);
        return enrollment;
    }

    public async Task<bool> CancelEnrollmentAsync(Guid enrollmentId, string? reason, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdWithDetailsAsync(enrollmentId, ct);
        if (enrollment == null) return false;

        _enrollmentRepo.Delete(enrollment);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}