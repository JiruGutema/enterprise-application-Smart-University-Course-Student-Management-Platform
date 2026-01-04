using Microsoft.EntityFrameworkCore;
using Courses.Domain.Entities;

namespace Courses.Infrastructure.Persistence;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
}
