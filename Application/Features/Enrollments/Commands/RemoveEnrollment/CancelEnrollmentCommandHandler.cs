using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
// يجب استبدال هذه الاستيرادات بالاستيرادات الصحيحة في مشروعك
// using Domain.Common.Interface; // لـ IUnitOfWork
// using Infrastructure.Interface; // لـ IEnrollmentRepository
using Microsoft.Extensions.Caching.Hybrid; // لـ HybridCache
using Microsoft.Extensions.Logging; // لـ Clean Code
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Commands.RemoveEnrollment
{
    // تم تغيير قيمة الإرجاع إلى bool
    public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand, bool>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly ILogger<CancelEnrollmentCommandHandler> _logger; // إضافة ILogger

        public CancelEnrollmentCommandHandler(
            IEnrollmentRepository enrollmentRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            ILogger<CancelEnrollmentCommandHandler> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelEnrollmentCommand request, CancellationToken ct)
        {
            try
            {
                // 1. جلب كيان التسجيل
                // (نفترض أن الدالة GetByIdAsync يمكنها قبول CancellationToken)
                var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, ct);

                if (enrollment == null)
                {
                    // Clean Code: رمي KeyNotFoundException (يعكس حالة 404)
                    _logger.LogWarning("Attempted to cancel non-existent enrollment ID: {Id}", request.EnrollmentId);
                    throw new KeyNotFoundException($"Enrollment with ID {request.EnrollmentId} not found.");
                }

                // 2. تنفيذ منطق المجال (Domain Logic)
                // يتم الآن افتراض أن الدالة enrollment.Cancel() ستقوم بما يلي:
                // أ) تحديث حالة الكيان (مثال: is_canceled = true)
                // ب) رمي استثناء (مثل InvalidOperationException) إذا كان الإلغاء غير مسموح به.
                enrollment.Cancel(request.CancellationReason);
                // **ملاحظة:** إذا فشل المنطق هنا، سيتم رمي استثناء يتم التقاطه في الـ Catch Block.

                // 3. التخزين المستمر (Persistence)
                await _enrollmentRepository.UpdateAsync(enrollment, ct);
                await _unitOfWork.CommitAsync(ct);

                // 4. مسح الذاكرة المؤقتة (Cache Management)
                // نفترض أن الخصائص Id, StudentId, CourseId متاحة في كيان enrollment
                await _cache.RemoveAsync($"Enrollment_{enrollment.Id}", ct);
                await _cache.RemoveByTagAsync($"Student_{enrollment.StudentId}", ct);
                await _cache.RemoveByTagAsync($"Course_{enrollment.CourseId}", ct);
                await _cache.RemoveByTagAsync("Enrollments", ct);

                _logger.LogInformation("Enrollment ID {Id} successfully canceled.", request.EnrollmentId);

                return true; // إرجاع النجاح
            }
            // 5. معالجة الاستثناءات (Try/Catch)
            catch (KeyNotFoundException)
            {
                // إعادة رمي الاستثناءات المعروفة ليتم التعامل معها في الـ Middleware
                throw;
            }
            catch (InvalidOperationException)
            {
                // إعادة رمي استثناءات منطق المجال (على افتراض أن الدالة Cancel ترميها)
                throw;
            }
            catch (Exception ex)
            {
                // تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "An unexpected error occurred during cancellation of enrollment {Id}.", request.EnrollmentId);
                throw new ApplicationException($"Failed to cancel enrollment {request.EnrollmentId}.", ex);
            }
        }
    }
}