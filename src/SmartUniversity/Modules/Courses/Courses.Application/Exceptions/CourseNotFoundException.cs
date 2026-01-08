using System;

namespace SmartUniversity.Modules.Courses.Application.Exceptions;

public class CourseNotFoundException : Exception
{
    public CourseNotFoundException(Guid courseId)
        : base($"Course with ID {courseId} was not found.") { }
}
