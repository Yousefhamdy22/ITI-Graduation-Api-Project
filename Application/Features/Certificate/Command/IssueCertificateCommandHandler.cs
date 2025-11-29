using Application.Features.Certificate.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using MediatR;

namespace Application.Features.Certificate.Command;

public class IssueCertificateCommandHandler : IRequestHandler<IssueCertificateCommand, CertificateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContext;

    public IssueCertificateCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<CertificateDto> Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
    {
        var role = _userContext.GetRole();

        if (role == "Student")
            throw new UnauthorizedAccessException("Students cannot issue certificates.");

        // جلب الـ user و course قبل أي عملية
        var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(request.UserId);
        if (user == null)
            throw new Exception($"User with ID {request.UserId} does not exist.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new UnauthorizedAccessException(role == "Instructor"
                ? "Instructor cannot issue this certificate."
                : "Course not found for Admin.");

        // التأكد إن الشهادة غير موجودة مسبقًا
        var existing = await _unitOfWork.Certificates.FindAsync(
            c => c.UserId == request.UserId && c.CourseId == request.CourseId,
            new[] { "User", "Course" });

        if (existing != null)
            return new CertificateDto
            {
                Id = existing.Id,
                UserName = existing.User.UserName,
                CourseName = existing.Course.Title,
                IssuedAt = existing.IssuedAt,
                CertificateNumber = existing.CertificateNumber
            };

        var certificate = new Core.Entities.Certificate
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId, 
            CourseId = request.CourseId,
            IssuedAt = DateTime.UtcNow,
            CertificateNumber = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Certificates.AddAsync(certificate);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return new CertificateDto
        {
            Id = certificate.Id,
            UserName = user.UserName,
            CourseName = course.Title,
            IssuedAt = certificate.IssuedAt,
            CertificateNumber = certificate.CertificateNumber
        };
    }
}