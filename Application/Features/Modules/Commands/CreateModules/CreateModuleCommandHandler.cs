using Application.Features.Modules.Dtos;
using AutoMapper;
using Core.Common.Results;
using Core.Entities.Courses;
using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Commands.CreateModules
{
    public class CreateModuleCommandHandler :IRequestHandler<CreateModuleCommand, Result<ModuleDto>>
{
        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;
       
        public CreateModuleCommandHandler(IModuleRepository moduleRepository ,
             IUnitOfWork unitOfWork, IMapper mapper , HybridCache cache)
        {
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<ModuleDto>> Handle(CreateModuleCommand request, CancellationToken ct)
        {
          
            var moduleResult = Module.Create(request.Title, request.Description, request.CourseId);

            if (moduleResult.IsError)
            {
                return Result<ModuleDto>.FromErrors(moduleResult.Errors);
            }

            var module = moduleResult.Value;

          
            await _moduleRepository.AddAsync(module);
            await _unitOfWork.CommitAsync(ct);

            var dto = _mapper.Map<ModuleDto>(module);

          
            string cacheKey = $"module:{dto.ModuleId}";
            await _cache.SetAsync(cacheKey, dto);
  
            await _cache.RemoveAsync("modules:all", ct);

            return Result<ModuleDto>.FromValue(dto);
        }
    }
}
