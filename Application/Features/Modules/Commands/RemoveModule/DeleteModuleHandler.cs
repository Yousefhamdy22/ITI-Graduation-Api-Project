using Core.Common.Results;
using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Commands.RemoveModule
{
    public class DeleteModuleHandler : IRequestHandler<DeleteModuleCommand, Result<bool>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly ILogger<DeleteModuleHandler> _logger;

        public DeleteModuleHandler(
            IModuleRepository moduleRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            ILogger<DeleteModuleHandler> logger)
        {
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteModuleCommand request, CancellationToken ct)
        {
            var module = await _moduleRepository.GetByIdAsync(request.ModuleId);

            if (module is null)
            {
                return Result<bool>.FromError(
                    Error.NotFound("Module.NotFound", "Module not found."));
            }

             _moduleRepository.Delete(module);
            await _unitOfWork.CommitAsync(ct);

            // Remove cache
            await _cache.RemoveAsync($"module:{request.ModuleId}", ct);
            await _cache.RemoveAsync("modules:all", ct);

            _logger.LogInformation("Module {ModuleId} deleted successfully.", module.Id);

            return Result<bool>.FromValue(true);
        }
    }

}
