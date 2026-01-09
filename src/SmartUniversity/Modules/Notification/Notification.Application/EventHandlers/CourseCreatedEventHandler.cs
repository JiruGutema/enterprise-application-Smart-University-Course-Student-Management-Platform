using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class CourseCreatedEventHandler
    {
        private readonly ILogger<CourseCreatedEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public CourseCreatedEventHandler(
            INotificationServices notificationServices,
            ILogger<CourseCreatedEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(CourseCreatedEvent evt)
        {
            _logger.LogInformation("CourseCreatedEvent received for Course {CourseId} - {Title}", evt.CourseId, evt.Title);

            // Create notification for the instructor
            string title = "Course Created Successfully";
            string message = $"Your course '{evt.Title}' ({evt.Code}) has been created successfully and is ready for configuration.";
            
            Notifications notification = new Notifications(
                evt.InstructorId,
                title,
                message,
                 Domain.Enums.NotificationType.Info
            );
            
            await _notificationServices.CreateNotificationAsync(notification);
        }
    }
}