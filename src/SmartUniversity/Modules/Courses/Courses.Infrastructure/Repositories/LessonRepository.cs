using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;

namespace SmartUniversity.Modules.Courses.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly CourseDbContext _dbContext;

    public LessonRepository(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _dbContext.Lessons.AddAsync(lesson);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lesson lesson)
    {
        _dbContext.Lessons.Remove(lesson);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Lesson?> GetByIdAsync(Guid id) => await _dbContext.Lessons.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Lesson>> GetByModuleIdAsync(Guid moduleId) => 
        await _dbContext.Lessons.Where(l => l.ModuleId == moduleId).OrderBy(l => l.Order).ToListAsync();

    public async Task UpdateAsync(Lesson lesson)
    {
        _dbContext.Lessons.Update(lesson);
        await _dbContext.SaveChangesAsync();
    }
}