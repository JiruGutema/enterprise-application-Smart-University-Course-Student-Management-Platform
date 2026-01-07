namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class PasswordChangedEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public PasswordChangedEvent(Guid userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}
