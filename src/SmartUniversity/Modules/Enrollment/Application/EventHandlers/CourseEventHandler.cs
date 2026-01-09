using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Shared.Kernel.Interface;
using SmartUniversity.Modules.Courses.Domain.Events;
using System.Threading;

namespace SmartUniversity.Modules.Enrollment.Application.EventHandlers
{
    public sealed class CourseEventHandler
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CourseEventHandler(
            IEnrollmentRepository enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleCourseDeletedAsync(CourseDeletedEvent evt)
        {
            var enrollments = await _enrollmentRepository.GetByCourseIdAsync(
                evt.CourseId,
                CancellationToken.None
            );

            foreach (var enrollment in enrollments)
            {
                enrollment.Drop(); // domain behavior only
            }

            // ✅ THIS is the only persistence boundary
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        // other handlers follow same pattern
    }
}
