using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Modules.Identity.Domain.Events;
namespace SmartUniversity.Modules.Identity.Domain.Entities;

using SmartUniversity.Shared.Kernel;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public Role Role { get; private set; } = Role.Student;
    public  string PasswordHash { get; private set; }
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

        AddDomainEvent(new UserRegisteredEvent(Id, Email, FullName));
    }

    public void ChangeRole(Role newRole)
    {
        Role = newRole;
        // Optionally raise UserRoleUpdatedEvent if it existed
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        AddDomainEvent(new UserEmailUpdatedEvent(Id, Email, FullName));
    }

    public void UpdateFullName(string fullName)
    {
        FullName = fullName;
        AddDomainEvent(new UserFullNameUpdatedEvent(Id, Email, FullName));
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        AddDomainEvent(new PasswordChangedEvent(Id, Email, FullName));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserAccountDeactivatedEvent(Id, Email, FullName));
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

    public void Delete()
    {
        AddDomainEvent(new UserDeletedEvent(Id, Email, FullName));
    }
}
