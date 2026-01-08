using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;

namespace SmartUniversity.Modules.Courses.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _dbContext;

    public CourseRepository(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetAllAsync() => await _dbContext.Courses.ToListAsync();

    public async Task<Course?> GetByIdAsync(Guid id) => await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Course>> GetByCodesAsync(IEnumerable<string> codes) => 
        await _dbContext.Courses.Where(c => codes.Contains(c.Code.Value)).ToListAsync();

    public async Task UpdateAsync(Course course)
    {
        _dbContext.Courses.Update(course);
        await _dbContext.SaveChangesAsync();
    }
}
