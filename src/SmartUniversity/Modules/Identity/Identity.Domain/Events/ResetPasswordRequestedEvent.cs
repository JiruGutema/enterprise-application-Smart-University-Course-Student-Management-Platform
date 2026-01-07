namespace SmartUniversity.Modules.Identity.Domain.Events
{
    public class ResetPasswordRequestedEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string ResetLink { get; }
        public string FullName { get; }

        public ResetPasswordRequestedEvent(
            Guid userId,
            string email,
            string fullName,
            string resetLink
        )
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            ResetLink = resetLink;
        }
    }
}
