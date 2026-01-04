using System;
using System.Threading.Tasks;

namespace Courses.Domain.Repository;
using Courses.Domain.Entities;
public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
