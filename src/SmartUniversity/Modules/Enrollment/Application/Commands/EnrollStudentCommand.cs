using MediatR;

namespace SmartUniversity.Modules.Enrollment.Application.Commands;

public record EnrollStudentCommand(Guid StudentId, Guid CourseId) : IRequest<Guid>;
