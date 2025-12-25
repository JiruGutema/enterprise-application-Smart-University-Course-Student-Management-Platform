namespace SmartUniversity.Modules.Identity.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}
