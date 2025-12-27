namespace SmartUniversity.Modules.Identity.Application.Interfaces
{
    public interface ICookieService
    {
        void SetLoginCookies(HttpResponse response, string accessToken, string refreshToken);
    }
}
