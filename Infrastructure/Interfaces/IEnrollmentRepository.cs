using Core.Interfaces;
using Core.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId,
            DateTimeOffset enrollmentDate, CancellationToken ct);

        Task<Enrollment?> GetByIdWithDetailsAsync(Guid enrollmentId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
        Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken ct = default);
        Task UpdateAsync(Enrollment e, CancellationToken ct = default);
    }
}
