using Core.Entities.Students;
using Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetWithEnrollmentsAsync(Guid studentId , CancellationToken ct = default);
        Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid studentId, CancellationToken ct = default);
        Task<Student?> GetByIdAsync(Guid studentId, CancellationToken ct = default);
        Task GetByUserIdAsnc(Guid userId, CancellationToken ct);
    }
}
