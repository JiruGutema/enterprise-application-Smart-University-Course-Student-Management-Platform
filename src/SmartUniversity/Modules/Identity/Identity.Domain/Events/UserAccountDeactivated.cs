namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class UserAccountDeactivatedEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserAccountDeactivatedEvent(
            Guid userId,
            string email,
            string fullName
        )
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}
