using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    /// <summary>
    /// Placeholder handler for future Grading module events
    /// When the Grading module implements domain events like AssignmentGradedEvent, GradeUpdatedEvent, etc.
    /// this handler can be extended to handle those events
    /// </summary>
    public class GradingEventHandler
    {
        private readonly ILogger<GradingEventHandler> _logger;
        private readonly INotificationServices _notificationServices;

        public GradingEventHandler(
            INotificationServices notificationServices,
            ILogger<GradingEventHandler> logger
        )
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        // Example method for future AssignmentGradedEvent
        // public async Task HandleAssignmentGradedAsync(AssignmentGradedEvent evt)
        // {
        //     _logger.LogInformation("AssignmentGradedEvent received for Student {StudentId} - Grade: {Grade}", 
        //         evt.StudentId, evt.Grade);
        //
        //     string title = "Assignment Graded";
        //     string message = $"Your assignment has been graded. Grade: {evt.Grade}";
        //     
        //     Notifications notification = new Notifications(
        //         evt.StudentId,
        //         title,
        //         message,
        //         Domain.Enums.NotificationType.Info
        //     );
        //     
        //     await _notificationServices.CreateNotificationAsync(notification);
        // }

        // Example method for future GradeUpdatedEvent
        // public async Task HandleGradeUpdatedAsync(GradeUpdatedEvent evt)
        // {
        //     _logger.LogInformation("GradeUpdatedEvent received for Student {StudentId} - New Grade: {NewGrade}", 
        //         evt.StudentId, evt.NewGrade);
        //
        //     string title = "Grade Updated";
        //     string message = $"Your grade has been updated. New grade: {evt.NewGrade}";
        //     
        //     Notifications notification = new Notifications(
        //         evt.StudentId,
        //         title,
        //         message,
        //         Domain.Enums.NotificationType.Info
        //     );
        //     
        //     await _notificationServices.CreateNotificationAsync(notification);
        // }
    }
}