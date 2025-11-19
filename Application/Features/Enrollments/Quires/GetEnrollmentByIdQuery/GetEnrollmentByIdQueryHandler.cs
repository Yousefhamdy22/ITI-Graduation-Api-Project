using Application.Features.Enrollments.Dto;
using AutoMapper;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // لـ KeyNotFoundException
using System.Threading;
using System.Threading.Tasks;
//... usings for IEnrollmentRepository, EnrollmentDetailsDto, Enrollment entity

namespace Application.Features.Enrollments.Quires.GetEnrollmentByIdQuery
{
    // تم تغيير قيمة الإرجاع إلى IRequestHandler<..., EnrollmentDetailsDto>
    public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDetailsDto>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEnrollmentByIdQueryHandler> _logger;

        public GetEnrollmentByIdQueryHandler(
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper,
            ILogger<GetEnrollmentByIdQueryHandler> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EnrollmentDetailsDto> Handle(GetEnrollmentByIdQuery request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve enrollment with ID: {Id}", request.EnrollmentId);

                // 1. جلب الكيان (يفترض أن IEnrollmentRepository.GetByIdAsync يمكنها قبول CancellationToken)
                var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, ct);

                if (enrollment == null)
                {
                    _logger.LogWarning("Enrollment ID {Id} not found.", request.EnrollmentId);
                    // رمي KeyNotFoundException بدلاً من Result.FromError
                    throw new KeyNotFoundException($"Enrollment with ID {request.EnrollmentId} not found.");
                }

                // 2. تعيين الكيان إلى DTO
                var dto = _mapper.Map<EnrollmentDetailsDto>(enrollment);

                _logger.LogInformation("Successfully retrieved enrollment ID: {Id}", request.EnrollmentId);

                // 3. إرجاع DTO
                return dto;
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error retrieving enrollment details for ID: {Id}", request.EnrollmentId);
                // رمي ApplicationException (أو استثناء مخصص)
                throw new ApplicationException($"Failed to retrieve enrollment details for ID {request.EnrollmentId}.", ex);
            }
        }
    }
}