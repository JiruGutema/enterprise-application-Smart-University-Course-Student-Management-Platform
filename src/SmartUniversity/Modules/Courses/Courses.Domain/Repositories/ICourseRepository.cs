using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartUniversity.Modules.Courses.Domain.Aggregates;

namespace SmartUniversity.Modules.Courses.Domain.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<IEnumerable<Course>> GetByCodesAsync(IEnumerable<string> codes);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
}

