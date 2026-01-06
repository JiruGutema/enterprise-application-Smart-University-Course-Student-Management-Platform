using SmartUniversity.Shared.Exceptions;
using System;

namespace SmartUniversity.Modules.Enrollment.Application.Exceptions
{
    public sealed class EnrollmentAlreadyExistsException : ApplicationExceptionBase
    {
        public override int StatusCode => 400;

        public EnrollmentAlreadyExistsException(Guid studentId, Guid courseId)
            : base($"Student {studentId} is already enrolled in course {courseId}.") { }

        public EnrollmentAlreadyExistsException()
            : base("Student is already enrolled in this course.") { }

        public EnrollmentAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
