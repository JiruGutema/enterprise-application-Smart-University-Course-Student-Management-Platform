using SmartUniversity.Modules.Identity.Domain.Enums;
namespace SmartUniversity.Modules.Identity.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public Role Role { get; private set; } = Role.Student;
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public User(Guid id, string email, string fullName, Role role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required");

        Id = id;
        Email = email;
        FullName = fullName;
        Role = role;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public void ChangeRole(Role newRole)
    {
        Role = newRole;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool IsInstructor()
    {
        return Role == Role.Instructor;
    }

    public bool IsAdmin()
    {
        return Role == Role.Admin;
    }
}
