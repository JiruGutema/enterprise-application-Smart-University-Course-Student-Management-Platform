using MediatR;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class DropEnrollmentCommand : IRequest
    {
        public Guid EnrollmentId { get; }

        public DropEnrollmentCommand(Guid enrollmentId)
        {
            EnrollmentId = enrollmentId;
        }
    }
}
