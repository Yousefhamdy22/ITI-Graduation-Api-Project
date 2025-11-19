using Application.Features.Enrollments.Dto;
using MediatR;

namespace Application.Features.Enrollments.Quires.GetEnrollmentByIdQuery
{
    public record GetEnrollmentByIdQuery(Guid EnrollmentId) : IRequest<EnrollmentDetailsDto>;
}
