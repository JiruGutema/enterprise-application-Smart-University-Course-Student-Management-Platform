using System;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Courses.Courses.Domain.Repository;

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
