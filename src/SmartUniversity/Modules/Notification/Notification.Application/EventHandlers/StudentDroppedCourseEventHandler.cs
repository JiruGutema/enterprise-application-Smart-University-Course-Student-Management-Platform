using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class StudentDroppedCourseEventHandler
    {
        private readonly ILogger<StudentDroppedCourseEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public StudentDroppedCourseEventHandler(
            INotificationServices notificationServices,
            ILogger<StudentDroppedCourseEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(StudentDroppedCourseEvent evt)
        {
            _logger.LogInformation("StudentDroppedCourseEvent received for Student {StudentId} dropping Course {CourseId}", 
                evt.StudentId, evt.CourseId);

            // Create notification for the student
            string title = "Course Dropped";
            string message = "You have successfully dropped the course. Your enrollment has been cancelled and you will no longer have access to course materials.";
            
            Notifications notification = new Notifications(
                evt.StudentId,
                title,
                message,
                Domain.Enums.NotificationType.Warning
            );
            
            await _notificationServices.CreateNotificationAsync(notification);
        }
    }
}