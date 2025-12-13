using Application.Features.Modules.Dtos;
using AutoMapper;
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

namespace Application.Features.Modules.Commands.UpdateModules
{
    public class UpdateModuleHandler : IRequestHandler<UpdateModuleCommand, Result<UpdatModuleDto>>
    {

        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;
        private readonly ILogger<UpdateModuleHandler> _logger;

        public UpdateModuleHandler(
            IModuleRepository moduleRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HybridCache cache,
            ILogger<UpdateModuleHandler> logger)
        {
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<UpdatModuleDto>> Handle(UpdateModuleCommand request, CancellationToken ct)
        {
            // Fetch the module by ID
            var module = await _moduleRepository.GetByIdAsync(request.Id);

            if (module == null)
            {
                return Result<UpdatModuleDto>.FromError(
                    Error.NotFound("Module.NotFound", "Module not found."));
            }

            // Domain validation and update
            module.Update(request.Title, request.Description);

            // Save the updated module
             _moduleRepository.Update(module);
            await _unitOfWork.CommitAsync(ct);

            // Map to DTO for return
            var dto = _mapper.Map<UpdatModuleDto>(module);

            // Update cache
            string cacheKey = $"module:{dto.ModuleId}";
            await _cache.SetAsync(cacheKey, dto);
            await _cache.RemoveAsync("modules:all", ct);

            // Log the update operation
            _logger.LogInformation("Module {ModuleId} updated successfully.", module.Id);

            return Result<UpdatModuleDto>.FromValue(dto);
        }
       
    }

}
