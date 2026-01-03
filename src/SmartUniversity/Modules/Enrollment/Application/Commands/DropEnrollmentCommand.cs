using MediatR;

namespace SmartUniversity.Modules.Enrollment.Application.Commands;

public record DropEnrollmentCommand(Guid EnrollmentId) : IRequest;
