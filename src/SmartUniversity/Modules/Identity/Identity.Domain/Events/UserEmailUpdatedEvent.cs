namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class UserEmailUpdatedEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserEmailUpdatedEvent(Guid userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}
