using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartUniversity.Modules.Courses.Domain.Aggregates;

namespace SmartUniversity.Modules.Courses.Domain.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(Guid id);
    Task<IEnumerable<Module>> GetByCourseIdAsync(Guid courseId);
    Task AddAsync(Module module);
    Task UpdateAsync(Module module);
    Task DeleteAsync(Module module);
}