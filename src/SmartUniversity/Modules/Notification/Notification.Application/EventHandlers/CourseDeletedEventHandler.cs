using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class CourseDeletedEventHandler
    {
        private readonly ILogger<CourseDeletedEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public CourseDeletedEventHandler(
            INotificationServices notificationServices,
            ILogger<CourseDeletedEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(CourseDeletedEvent evt)
        {
            _logger.LogInformation("CourseDeletedEvent received for Course {CourseId}", evt.CourseId);

            // Note: In a real implementation, you would need to get affected users (enrolled students, instructor)
            // and notify them about the course deletion
            // For now, we'll log the event
            _logger.LogInformation("Course {CourseId} deleted - notifications would be sent to affected users", evt.CourseId);

            // This would typically require:
            // 1. Get all enrolled students for this course
            // 2. Get the instructor of this course
            // 3. Send notifications to all affected users
            // 
            // Example notification:
            // string title = "Course Cancelled";
            // string message = "The course you were enrolled in has been cancelled. Please contact administration for more information.";
            // 
            // foreach (var studentId in enrolledStudents)
            // {
            //     Notifications notification = new Notifications(
            //         studentId,
            //         title,
            //         message,
            //         Domain.Enums.NotificationType.Warning
            //     );
            //     await _notificationServices.CreateNotificationAsync(notification);
            // }
        }
    }
}