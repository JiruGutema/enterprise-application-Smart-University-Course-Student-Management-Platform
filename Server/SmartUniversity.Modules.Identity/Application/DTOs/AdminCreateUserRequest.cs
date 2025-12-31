using SmartUniversity.Modules.Identity.Domain.Enums;

namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class AdminCreateUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required Role Role { get; set; } 

    }
}
