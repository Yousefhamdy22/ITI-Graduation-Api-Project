using Core.Entities.Students;
using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Common.GenRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
 public class StudentRepository : GenericRepository<Student>, IStudentRepository
 {
 private readonly AppDBContext _context;
 public StudentRepository(AppDBContext context) : base(context)
 {
 _context = context;
 }

 public async Task<Student?> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct = default)
 {
 return await _context.Students
 .Include(s => s.Enrollments)
 .FirstOrDefaultAsync(s => s.Id == studentId, ct);
 }

 public async Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct = default)
 {
 return await _context.Enrollments
 .Where(e => e.CourseId == courseId)
 .Select(e => e.Student)
 .ToListAsync(ct);
 }

 public async Task<bool> ExistsAsync(Guid studentId, CancellationToken ct = default)
 {
 return await _context.Students.AnyAsync(s => s.Id == studentId, ct);
 }

 public async Task<Student?> GetByIdAsync(Guid studentId, CancellationToken ct = default)
 {
 return await _context.Students.FindAsync(new object[] { studentId }, ct);
 }

        Task IStudentRepository.GetByUserIdAsnc(Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
