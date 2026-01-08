using MediatR;
using SmartUniversity.Modules.Courses.Application.DTOs;

namespace SmartUniversity.Modules.Courses.Application.Queries;

public record GetCourseMetadataQuery(Guid CourseId) : IRequest<CourseResponse>;