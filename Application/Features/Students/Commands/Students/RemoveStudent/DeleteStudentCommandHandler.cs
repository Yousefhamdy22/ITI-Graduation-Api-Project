using Core.Interfaces;
using Infrastructure.Common.GenRepo;

// تم حذف using Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // لـ KeyNotFoundException
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Students.Commands.Students.RemoveStudent
{
    // 1. تغيير قيمة الإرجاع من Result<bool> إلى bool
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, bool>
    {
        // تم تغيير اسم الواجهة إلى IGenericRepository للامتثال لما هو مكتوب لديك
        private readonly IGenericRepository<Core.Entities.Students.Student> _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteStudentCommandHandler> _logger;
        private readonly HybridCache _cache;

        public DeleteStudentCommandHandler(
            GenericRepository<Core.Entities.Students.Student> studentRepository,
            ILogger<DeleteStudentCommandHandler> logger,
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        // 2. تغيير قيمة الإرجاع إلى Task<bool>
        public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Deleting student {StudentId}", request.StudentId);

                var student = await _studentRepository.GetByIdAsync(request.StudentId, ct);

                if (student == null)
                {
                    _logger.LogWarning("Student {StudentId} not found", request.StudentId);
                    // 3. رمي KeyNotFoundException بدلاً من Result.FromError(Error.NotFound)
                    throw new KeyNotFoundException($"Student with ID {request.StudentId} not found.");
                }

                // Check if student has enrollments or exam results
                if (student.Enrollments.Any() || student.ExamResults.Any())
                {
                    _logger.LogWarning("Cannot delete student {StudentId} - has associated enrollments or exam results",
                        request.StudentId);
                    // 4. رمي InvalidOperationException بدلاً من Result.FromError(Error.Conflict)
                    throw new InvalidOperationException("Cannot delete student with existing enrollments or exam results.");
                }

                await _studentRepository.DeleteAsync(student, ct);
                await _unitOfWork.CommitAsync(ct);

                // Invalidate cache
                await _cache.RemoveByTagAsync($"student:{request.StudentId}", ct);
                await _cache.RemoveByTagAsync("students", ct);

                _logger.LogInformation("Student {StudentId} deleted successfully", request.StudentId);

                // 5. إرجاع القيمة مباشرةً
                return true;
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (InvalidOperationException)
            {
                // إعادة رمي استثناءات منطق المجال والتضارب
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error deleting student with ID: {Id}", request.StudentId);
                throw new ApplicationException($"An unexpected error occurred while deleting student {request.StudentId}.", ex);
            }
        }
    }
}