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
 public class AnswerOptionRepository : GenericRepository<AnswerOption>, IAnswerOptionRepository
 {
 private readonly AppDBContext _context;
 public AnswerOptionRepository(AppDBContext context) : base(context)
 {
 _context = context;
 }

 public async Task<List<AnswerOption>> GetByQuestionIdAsync(Guid questionId, CancellationToken ct = default)
 {
 return await _context.AnswerOptions.Where(a => a.QuestionId == questionId).ToListAsync(ct);
 }

 public async Task<AnswerOption?> GetByIdAsync(Guid id, CancellationToken ct = default)
 {
 return await _context.AnswerOptions.FindAsync(new object[] { id }, ct);
 }
 }
}
