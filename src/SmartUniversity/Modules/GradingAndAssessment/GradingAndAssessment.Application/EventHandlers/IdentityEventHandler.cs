using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Cache;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;
using SmartUniversity.Modules.Identity.Domain.Events;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.EventHandlers;

public class IdentityEventHandler
{
    private readonly GradingDbContext _context;

    public IdentityEventHandler(GradingDbContext context)
    {
        _context = context;
    }

    public async Task HandleUserRegisteredAsync(UserRegisteredEvent userRegisteredEvent)
    {
        var existingStudent = await _context.StudentCache
            .FirstOrDefaultAsync(s => s.StudentId == userRegisteredEvent.UserId);

        if (existingStudent == null)
        {
            var studentCache = new StudentCache
            {
                StudentId = userRegisteredEvent.UserId,
                FullName = userRegisteredEvent.FullName,
                Email = userRegisteredEvent.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.StudentCache.Add(studentCache);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update existing record
            existingStudent.FullName = userRegisteredEvent.FullName;
            existingStudent.Email = userRegisteredEvent.Email;
            existingStudent.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}