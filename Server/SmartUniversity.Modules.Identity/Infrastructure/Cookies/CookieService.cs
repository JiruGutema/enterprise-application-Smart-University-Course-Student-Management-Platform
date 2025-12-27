using SmartUniversity.Modules.Identity.Application.Interfaces;

public class CookieService : ICookieService
{
    public void SetLoginCookies(HttpResponse response, string accessToken, string refreshToken)
    {
        response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
            }
        );

        response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(14),
            }
        );
    }
}
