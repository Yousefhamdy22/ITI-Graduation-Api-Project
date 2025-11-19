using Core.Entities.Exams;
using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Common.GenRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
 public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository
 {
 private readonly AppDBContext _context;
 public ExamResultRepository(AppDBContext context) : base(context)
 {
 _context = context;
 }

 public async Task<ExamResult?> GetByStudentAndExamAsync(Guid studentId, Guid examId, CancellationToken ct = default)
 {
 return await _context.ExamResults.FirstOrDefaultAsync(er => er.StudentId == studentId && er.ExamId == examId, ct);
 }

 public async Task<List<ExamResult>> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default)
 {
 return await _context.ExamResults.Where(er => er.StudentId == studentId).ToListAsync(ct);
 }

 public async Task<ExamResult?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
 {
 return await _context.ExamResults
 .Include(er => er.Student)
 .Include(er => er.StudentAnswers)
 .FirstOrDefaultAsync(er => er.Id == id, ct);
 }

 public async Task UpdateAsync(ExamResult examResult, CancellationToken ct = default)
 {
 _context.ExamResults.Update(examResult);
 await Task.CompletedTask;
 }

 public async Task<ExamResult?> GetByIdAsync(Guid examResultId, CancellationToken ct = default)
 {
 return await _context.ExamResults.FindAsync(new object[] { examResultId }, ct);
 }
 }
}
