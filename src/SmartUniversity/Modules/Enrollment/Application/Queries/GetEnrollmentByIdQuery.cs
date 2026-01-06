using MediatR;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Queries
{
    public class GetEnrollmentByIdQuery : IRequest<EnrollmentDetailsResponse?>
    {
        public Guid EnrollmentId { get; }

        public GetEnrollmentByIdQuery(Guid enrollmentId)
        {
            EnrollmentId = enrollmentId;
        }
    }
}
