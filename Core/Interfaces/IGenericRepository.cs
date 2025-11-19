using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Entities.Students;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(Guid id);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    T? Find(Expression<Func<T, bool>> criteria, string[]? includes = null);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[]? includes = null, CancellationToken ct = default);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[]? includes = null);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int skip, int take);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
        Expression<Func<T, object>>? orderBy = null, string orderByDirection = "ASC");

    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int skip, int take, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
        Expression<Func<T, object>>? orderBy = null, string orderByDirection = "ASC", CancellationToken ct = default);

    T Add(T entity);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    IEnumerable<T> AddRange(IEnumerable<T> entities);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    T Update(T entity);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    void Attach(T entity);
    void AttachRange(IEnumerable<T> entities);
    int Count();
    int Count(Expression<Func<T, bool>> criteria);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>> criteria, CancellationToken ct = default);
    Task DeleteAsync(Student student, CancellationToken ct);
}