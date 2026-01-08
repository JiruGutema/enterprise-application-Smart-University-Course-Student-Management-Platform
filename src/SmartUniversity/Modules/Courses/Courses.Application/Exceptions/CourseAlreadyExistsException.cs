using System;

namespace SmartUniversity.Modules.Courses.Application.Exceptions;

public class CourseAlreadyExistsException : Exception
{
    public CourseAlreadyExistsException(string code)
        : base($"Course with code '{code}' already exists.") { }
}
