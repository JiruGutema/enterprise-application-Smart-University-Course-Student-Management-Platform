using MediatR;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class DropEnrollmentCommand : IRequest
{
    public Guid EnrollmentId { get; }
    public Guid ActorUserId { get; }
    public bool IsAdmin { get; }

    public DropEnrollmentCommand(Guid enrollmentId, Guid actorUserId, bool isAdmin)
    {
        EnrollmentId = enrollmentId;
        ActorUserId = actorUserId;
        IsAdmin = isAdmin;
    }
}

}
