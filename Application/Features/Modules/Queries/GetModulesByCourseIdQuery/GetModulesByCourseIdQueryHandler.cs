
using Application.Features.Lectures.Dtos;
using Application.Features.Modules.Dtos;
using Application.Features.Modules.Queries.GetModulesQuery;
using AutoMapper;
using Core.Common.Results;
using Core.Interfaces;
using Infrastructure.Interface;
using Infrastructure.Specifications;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Queries.GetModulesByCourseIdQuery_
{
    public record GetModulesByCourseIdQueryHandler : IRequestHandler<GetModulesByCourseIdQuery, Result<List<ModuleDto>>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IGenericRepository<Core.Entities.Courses.Module> _genericModuleRepository;
        private readonly HybridCache _cache;
        private readonly ILogger<GetModulesByCourseIdQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetModulesByCourseIdQueryHandler(
            IModuleRepository moduleRepository,
            HybridCache cache,
            ILogger<GetModulesByCourseIdQueryHandler> logger,
            IGenericRepository<Core.Entities.Courses.Module> genericModuleRepository,
            IMapper mapper)
        {
            _moduleRepository = moduleRepository;
            _genericModuleRepository = genericModuleRepository;
            _cache = cache;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<List<ModuleDto>>> Handle(GetModulesByCourseIdQuery request, CancellationToken ct)
        {
            try
            {
                var cacheKey = $"modules_course_{request.CourseId}";
                var cacheTags = new[] { "modules", $"course_{request.CourseId}" };

                var modules = await _cache.GetOrCreateAsync(
                    cacheKey,
                    async cancellationToken =>
                    {
                        _logger.LogInformation("Cache miss - loading modules for course {CourseId} from database", request.CourseId);

                        // need fiexable INclude with expression 
                        //var modulesWithLectures = await _moduleRepository
                        //    .GetAllWithSpecAsync(m => m.CourseId == request.CourseId, include: "Lectures");
                        // getall with Query and Include expression 

                        var spec = new ModulesByCourseIdWithLecturesSpecification(request.CourseId);
                        var entities = await _moduleRepository.GetAllWithSpecAsync(spec, ct);


                        if (entities == null || !entities.Any())
                            return new List<ModuleDto>();

                        return entities.Select(m => new ModuleDto
                        {
                            ModuleId = m.Id,
                            Title = m.Title,
                            Description = m.Description,
                            CourseId = m.CourseId,
                            Lectures = m.Lectures.Select(l => new ResponseLecture
                            {
                                LectureId = l.Id,
                                Title = l.Title,
                                ScheduledAt = l.ScheduledAt,
                                Duration = l.Duration,
                                ZoomMeeting = l.ZoomMeeting != null ? new ZoomMeetingDto
                                {
                                    // Map ZoomMeeting properties here as needed
                                } : null
                            }).ToList()
                        }).ToList();
                    },
                    options: new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(30)
                    },
                    tags: cacheTags,
                    cancellationToken: ct);

                return Result<List<ModuleDto>>.FromValue(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modules for course {CourseId}", request.CourseId);
                return Result<List<ModuleDto>>.FromError(Error.Failure("Module.RetrievalFailed", ex.Message));
            }
        }
    }

}
