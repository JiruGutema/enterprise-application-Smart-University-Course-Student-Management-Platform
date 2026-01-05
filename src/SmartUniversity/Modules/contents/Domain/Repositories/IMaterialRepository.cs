using SmartUniversity.Modules.Content.Domain.Entities;

namespace SmartUniversity.Modules.Content.Domain.Repositories;

public interface IMaterialRepository
{
    Task AddAsync(Material material);
}
