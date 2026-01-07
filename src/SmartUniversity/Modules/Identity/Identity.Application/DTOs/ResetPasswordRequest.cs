namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class ResetPasswordRequest
    {
        public required string ResetToken { get; init; }
        public required string NewPassword { get; init; }
    }
}
