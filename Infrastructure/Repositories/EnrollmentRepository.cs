using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Courses;
using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Common.GenRepo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
 public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
 {
 private readonly AppDBContext _context;
 public EnrollmentRepository(AppDBContext context) : base(context)
 {
 _context = context;
 }

 public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, DateTimeOffset enrollmentDate, CancellationToken ct)
 {
 return await _context.Enrollments
 .AsNoTracking()
 .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
 }

 public async Task<Enrollment?> GetByIdWithDetailsAsync(Guid enrollmentId, CancellationToken ct = default)
 {
 return await _context.Enrollments
 .Include(e => e.Student)
 .Include(e => e.Course)
 .FirstOrDefaultAsync(e => e.Id == enrollmentId, ct);
 }

 public async Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
 {
 return await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
 }

 public async Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken ct = default)
 {
 await _context.Enrollments.AddAsync(enrollment, ct);
 return enrollment;
 }

 public async Task UpdateAsync(Enrollment e, CancellationToken ct = default)
 {
 _context.Enrollments.Update(e);
 await Task.CompletedTask;
 }
 }
}
