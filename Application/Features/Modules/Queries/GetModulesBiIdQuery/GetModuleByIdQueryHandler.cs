
using Application.Features.Modules.Dtos;
using AutoMapper;
using Core.Common.Results;
using Core.Interfaces;

using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;


namespace Application.Features.Modules.Queries.GetModulesBiIdQuery
{
    public class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, Result<ModuleDto>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;
        private readonly ILogger<GetModuleByIdQueryHandler> _logger;

        public GetModuleByIdQueryHandler(
            IModuleRepository moduleRepository,
            IMapper mapper,
            HybridCache cache,
            ILogger<GetModuleByIdQueryHandler> logger)
        {
            _moduleRepository = moduleRepository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<ModuleDto>> Handle(GetModuleByIdQuery request, CancellationToken ct)
        {
            try
            {
                var cacheKey = $"module_{request.ModuleId}";
                var cacheTags = new[] { "modules", $"module_{request.ModuleId}" };

                var moduleDto = await _cache.GetOrCreateAsync(
                    cacheKey,
                    async cancellationToken =>
                    {
                        _logger.LogInformation("Cache miss - fetching module {ModuleId}", request.ModuleId);

                        var module = await _moduleRepository.GetModuleWithLecturesAsync(request.ModuleId, ct);
                        if (module == null)
                            return null;

                        var dto = _mapper.Map<ModuleDto>(module);
                        return dto;
                    },
                    options: new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(60)
                    },
                    tags: cacheTags,
                    cancellationToken: ct
                );

                if (moduleDto == null)
                {
                    _logger.LogWarning("Module {ModuleId} not found", request.ModuleId);
                    return Result<ModuleDto>.FromError(Error.NotFound("Module not found"));
                }

                _logger.LogInformation("Successfully retrieved module {ModuleId}", request.ModuleId);
                return Result<ModuleDto>.FromValue(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting module by ID {ModuleId}", request.ModuleId);
                return Result<ModuleDto>.FromError(Error.Failure("Module.RetrievalFailed", ex.Message));
            }
        }
    }

}
