using Core.Entities.Students;
using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Common.GenRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
 public class StudentAnswerRepository : GenericRepository<StudentAnswer>, IStudentAnswerRepository
 {
 private readonly AppDBContext _context;
 public StudentAnswerRepository(AppDBContext context) : base(context)
 {
 _context = context;
 }

 public async Task<StudentAnswer> AddAsync(StudentAnswer entity, CancellationToken ct = default)
 {
 await _context.StudentAnswers.AddAsync(entity, ct);
 return entity;
 }

 public async Task<StudentAnswer?> GetByIdAsync(Guid id, CancellationToken ct = default)
 {
 return await _context.StudentAnswers.FindAsync(new object[] { id }, ct);
 }
 }
}
