using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using SmartUniversity.Modules.Courses.Application.Services;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;
using SmartUniversity.Modules.Courses.Infrastructure.Repositories;

namespace SmartUniversity.Modules.Courses;

public static class CoursesModule
{
    public static IServiceCollection AddCoursesModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1️⃣ Register DbContext
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<CourseDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // 2️⃣ Register Repositories
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();

        // 3️⃣ Register Services
        services.AddScoped<CourseService>();

        // 4️⃣ Register MediatR handlers (commands & queries)
        services.AddMediatR(typeof(CoursesModule).Assembly);

        return services;
    }
}
