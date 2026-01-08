using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public record PublishCourseCommand(Guid CourseId) : IRequest;
