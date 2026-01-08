using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public record ReassignInstructorCommand(Guid CourseId, Guid NewInstructorId) : IRequest;
