
namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public required string Email { get; init; }
        public required string FullName { get; init; }
        public required string Role { get; set; }
        public required bool IsActive { get; set; }
    }
}
