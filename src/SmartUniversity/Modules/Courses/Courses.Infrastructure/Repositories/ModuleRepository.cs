using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;

namespace SmartUniversity.Modules.Courses.Infrastructure.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly CourseDbContext _dbContext;

    public ModuleRepository(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Module module)
    {
        await _dbContext.Modules.AddAsync(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Module module)
    {
        _dbContext.Modules.Remove(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Module?> GetByIdAsync(Guid id) => await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IEnumerable<Module>> GetByCourseIdAsync(Guid courseId) => 
        await _dbContext.Modules.Where(m => m.CourseId == courseId).OrderBy(m => m.Order).ToListAsync();

    public async Task UpdateAsync(Module module)
    {
        _dbContext.Modules.Update(module);
        await _dbContext.SaveChangesAsync();
    }
}