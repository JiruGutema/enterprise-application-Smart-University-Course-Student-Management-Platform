using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class EnrollmentStatusChangedEventHandler
    {
        private readonly ILogger<EnrollmentStatusChangedEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public EnrollmentStatusChangedEventHandler(
            INotificationServices notificationServices,
            ILogger<EnrollmentStatusChangedEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(EnrollmentStatusChangedEvent evt)
        {
            _logger.LogInformation("EnrollmentStatusChangedEvent received for Enrollment {EnrollmentId} - New Status: {NewStatus}", 
                evt.EnrollmentId, evt.NewStatus);

            // Note: We need the StudentId to create the notification
            // In a real implementation, you would need to look up the enrollment to get the StudentId
            // For now, we'll log the event
            _logger.LogInformation("Enrollment {EnrollmentId} status changed to {NewStatus} - notification would be sent to student", 
                evt.EnrollmentId, evt.NewStatus);

            // This would require a lookup service to get the StudentId from EnrollmentId
            // string title = "Enrollment Status Updated";
            // string message = $"Your enrollment status has been updated to: {evt.NewStatus}";
            // 
            // Notifications notification = new Notifications(
            //     studentId, // Need to lookup studentId from enrollmentId
            //     title,
            //     message,
            //     Domain.Enums.NotificationType.Info
            // );
            // 
            // await _notificationServices.CreateNotificationAsync(notification);
        }
    }
}