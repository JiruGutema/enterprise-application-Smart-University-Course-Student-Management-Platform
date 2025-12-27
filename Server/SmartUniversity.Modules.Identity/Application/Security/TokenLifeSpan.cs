namespace SmartUniversity.Modules.Identity.Application.Security;

public static class TokenLifetimes
{
    public static readonly TimeSpan Access = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan Refresh = TimeSpan.FromDays(14);
    public static readonly TimeSpan EmailVerification = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan PasswordReset = TimeSpan.FromMinutes(15);
}
