using MediatR;
using SmartUniversity.Modules.Courses.Application.DTOs;

namespace SmartUniversity.Modules.Courses.Application.Queries;

public record GetCoursesByCodesQuery(string[] Codes) : IRequest<IEnumerable<CourseResponse>>;
