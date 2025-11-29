using Application.Features.Certificate.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using MediatR;

namespace Application.Features.Certificate.Query;

public class ViewMyCertificateQueryHandler : IRequestHandler<ViewMyCertificateQuery, List<CertificateDto>>
{
    private readonly IGenericRepository<Core.Entities.Certificate> _certificateRepo;
    private readonly IUserContextService _userContext;

    public ViewMyCertificateQueryHandler(IGenericRepository<Core.Entities.Certificate> certificateRepo,
        IUserContextService userContext)
    {
        _certificateRepo = certificateRepo;
        _userContext = userContext;
    }

    public async Task<List<CertificateDto>> Handle(ViewMyCertificateQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var certificates = await _certificateRepo.FindAllAsync(
            c => c.UserId == userId,
            new[] { "User", "Course" }
        );

        if (certificates == null || !certificates.Any()) return new List<CertificateDto>();

        var dtos = certificates.Select(c => new CertificateDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserName = c.User?.FullName ?? "Unknown User",
            CourseName = c.Course?.Title ?? "Unknown Course",
            IssuedAt = c.IssuedAt,
            CertificateNumber = c.CertificateNumber
        }).ToList();

        return dtos;
    }
}