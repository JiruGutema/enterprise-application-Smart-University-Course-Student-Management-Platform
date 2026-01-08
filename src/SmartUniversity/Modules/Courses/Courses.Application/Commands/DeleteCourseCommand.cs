using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public record DeleteCourseCommand(Guid CourseId) : IRequest;
