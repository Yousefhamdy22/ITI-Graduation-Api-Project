using Core.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IModuleRepository : IGenericRepository<Module>
    {
        Task<Module?> GetModuleWithLecturesAsync(Guid moduleId, CancellationToken ct);
    }
}
