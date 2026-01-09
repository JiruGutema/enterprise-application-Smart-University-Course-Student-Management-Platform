using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class CoursePublishedEventHandler
    {
        private readonly ILogger<CoursePublishedEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public CoursePublishedEventHandler(
            INotificationServices notificationServices,
            ILogger<CoursePublishedEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(CoursePublishedEvent evt)
        {
            _logger.LogInformation("CoursePublishedEvent received for Course {CourseId}", evt.CourseId);

            // Note: In a real implementation, you would need to get the instructor ID from the course
            // For now, we'll create a system notification that can be sent to relevant users
            string title = "Course Published";
            string message = $"A course has been published and is now available for enrollment.";
            
            // This would typically be sent to administrators or relevant stakeholders
            // You might need to implement a way to get relevant user IDs for this notification
            _logger.LogInformation("Course {CourseId} published - notification logic would send to relevant users", evt.CourseId);
        }
    }
}