using MediatR;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Queries
{
    public class GetEnrollmentByIdQuery : IRequest<EnrollmentDetailsResponse?>
{
    public Guid EnrollmentId { get; }
    public Guid UserId { get; }
    public IReadOnlyCollection<string> Roles { get; }

    public GetEnrollmentByIdQuery(
        Guid enrollmentId,
        Guid userId,
        IReadOnlyCollection<string> roles)
    {
        EnrollmentId = enrollmentId;
        UserId = userId;
        Roles = roles;
    }
}
}
