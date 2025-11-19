using Application.Features.Enrollments.Commands.UpdateEnrollment;
using Application.Features.Enrollments.Dto;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
//... usings for IEnrollmentRepository, IUnitOfWork, EnrollmentDto, Enrollment entity

namespace Application.Features.Enrollments.Commands.UpdateEnrollmentStatus
{
    // تم تغيير قيمة الإرجاع من IRequestHandler<..., Result<EnrollmentDto>> إلى IRequestHandler<..., EnrollmentDto>
    public class UpdateEnrollmentStatusCommandHandler : IRequestHandler<UpdateEnrollmentStatusCommand, EnrollmentDto>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateEnrollmentStatusCommandHandler> _logger;

        public UpdateEnrollmentStatusCommandHandler(
            IEnrollmentRepository enrollmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateEnrollmentStatusCommandHandler> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EnrollmentDto> Handle(UpdateEnrollmentStatusCommand request, CancellationToken ct)
        {
            try
            {
                // 1. جلب الكيان
                var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, ct);

                if (enrollment == null)
                {
                    _logger.LogWarning("Enrollment ID {Id} not found.", request.EnrollmentId);
                    // رمي استثناء Not Found
                    throw new KeyNotFoundException($"Enrollment with ID {request.EnrollmentId} not found.");
                }

                // 2. تنفيذ منطق المجال لتحديث الحالة (نفترض وجود دالة SetStatus في الكيان)
                // Clean Code: يجب أن تكون الدالة SetStatus هي المسؤولة عن التحقق من صلاحية الحالة الجديدة

                // نفترض أن هذه الدالة تحدث الكيان وترمي استثناء إذا فشلت
                enrollment.SetStatus(request.Status, request.Reason);

                // 3. التخزين المستمر
                await _enrollmentRepository.UpdateAsync(enrollment, ct);
                await _unitOfWork.CommitAsync(ct);

                _logger.LogInformation("Enrollment ID {Id} status updated to {Status}.",
                                       request.EnrollmentId, request.Status);

                // 4. تعيين الكيان إلى DTO وإرجاعه
                var dto = _mapper.Map<EnrollmentDto>(enrollment);
                return dto;
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (InvalidOperationException)
            {
                // إعادة رمي استثناءات منطق المجال (مثل محاولة الانتقال لحالة غير مسموح بها)
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error updating enrollment status for ID: {Id}", request.EnrollmentId);
                throw new ApplicationException($"Failed to update status for enrollment {request.EnrollmentId}.", ex);
            }
        }
    }
}