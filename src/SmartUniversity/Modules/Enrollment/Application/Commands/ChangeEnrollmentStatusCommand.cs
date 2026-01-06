using MediatR;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class ChangeEnrollmentStatusCommand : IRequest
    {
        public Guid EnrollmentId { get; }
        public string Status { get; }

        public ChangeEnrollmentStatusCommand(Guid enrollmentId, string status)
{
    EnrollmentId = enrollmentId;
    Status = status;
}

    }
}
