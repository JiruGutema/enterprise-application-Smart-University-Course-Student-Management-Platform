using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartUniversity.Modules.Courses.Domain.Aggregates;

namespace SmartUniversity.Modules.Courses.Domain.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id);
    Task<IEnumerable<Lesson>> GetByModuleIdAsync(Guid moduleId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(Lesson lesson);
}