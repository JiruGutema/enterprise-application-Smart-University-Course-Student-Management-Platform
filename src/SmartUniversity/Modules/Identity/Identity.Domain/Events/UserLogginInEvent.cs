namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class UserLoggedInEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }
        public string Location { get; }
        public DateTime LoginTime { get; }

        public UserLoggedInEvent(
            Guid userId,
            string email,
            string fullName,
            string location,
            DateTime loginTime
        )
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            Location = location;
            LoginTime = loginTime;
        }
    }
}
