using Core.Entities.Courses;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetWithLecturesAsync(Guid courseId, CancellationToken ct = default);
        Task<IEnumerable<Course>> GetActiveCoursesAsync(CancellationToken ct = default);

    }
}
