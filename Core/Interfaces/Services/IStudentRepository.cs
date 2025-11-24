using Core.Entities.Students;

namespace Core.Interfaces.Services;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct = default);
    public Task<Student?> GetByIdWithUserAsync(Guid id, CancellationToken ct);
    public Task<List<Student>> GetAllWithEnrollmentsAsync(CancellationToken ct = default);
    Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid courseId);

    Task<bool> ExistsAsync(Guid studentId, CancellationToken ct = default);
}