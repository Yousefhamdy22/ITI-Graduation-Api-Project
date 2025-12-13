using Core.Entities.Courses;
using Core.Interfaces;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.services
{
    public class ModuleRepository : GenericRepository<Module>, IModuleRepository
    {
        private readonly AppDBContext _context;

        public ModuleRepository(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Module?> GetModuleWithLecturesAsync(Guid moduleId, CancellationToken ct)
        {
            return await _context.Modules
                .Include(m => m.Lectures)
                    .ThenInclude(l => l.ZoomMeeting)
                        .ThenInclude(z => z.Recordings).AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == moduleId, ct);
        }
    }
}
