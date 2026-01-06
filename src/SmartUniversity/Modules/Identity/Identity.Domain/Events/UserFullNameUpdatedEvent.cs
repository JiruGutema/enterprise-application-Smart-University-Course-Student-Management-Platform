
namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class UserFullNameUpdatedEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserFullNameUpdatedEvent(Guid userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}
