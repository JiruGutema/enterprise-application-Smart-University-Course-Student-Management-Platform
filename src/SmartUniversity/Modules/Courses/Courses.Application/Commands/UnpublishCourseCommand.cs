using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public record UnpublishCourseCommand(Guid CourseId) : IRequest;
