using SmartUniversity.Modules.Identity.Domain.Enums;

namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; init; } = null!;
        public string FullName { get; init; } = null!;
        public Role Role { get; set; }
        public bool IsActive { get; set; }
    }
}
