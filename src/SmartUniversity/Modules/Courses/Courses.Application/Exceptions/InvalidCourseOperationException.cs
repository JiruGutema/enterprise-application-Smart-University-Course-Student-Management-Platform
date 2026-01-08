using System;

namespace SmartUniversity.Modules.Courses.Application.Exceptions;

public class InvalidCourseOperationException : Exception
{
    public InvalidCourseOperationException(string message)
        : base(message) { }
}
