using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class StudentEnrolledEventHandler
    {
        private readonly ILogger<StudentEnrolledEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public StudentEnrolledEventHandler(
            INotificationServices notificationServices,
            ILogger<StudentEnrolledEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(StudentEnrolledEvent evt)
        {
            _logger.LogInformation("StudentEnrolledEvent received for Student {StudentId} in Course {CourseId}", 
                evt.StudentId, evt.CourseId);

            // Create notification for the student
            string title = "Enrollment Successful";
            string message = "You have been successfully enrolled in the course. You can now access course materials and assignments.";
            
            Notifications notification = new Notifications(
                evt.StudentId,
                title,
                message,
                 Domain.Enums.NotificationType.Info
            );
            
            await _notificationServices.CreateNotificationAsync(notification);
        }
    }
}