using MediatR;
using SmartUniversity.Modules.Courses.Application.DTOs;

namespace SmartUniversity.Modules.Courses.Application.Queries;

public record GetCourseByIdQuery(Guid CourseId) : IRequest<CourseResponse>;
