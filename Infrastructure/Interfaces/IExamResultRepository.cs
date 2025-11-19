using Core.Entities.Exams;
using Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IExamResultRepository : IGenericRepository<ExamResult>
    {
        Task<ExamResult?> GetByStudentAndExamAsync(Guid studentId, Guid examId, CancellationToken ct = default);
        Task<List<ExamResult>> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default);
        Task<ExamResult?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(ExamResult examResult, CancellationToken ct = default);
        Task<ExamResult?> GetByIdAsync(Guid examResultId, CancellationToken ct = default);
    }
}
