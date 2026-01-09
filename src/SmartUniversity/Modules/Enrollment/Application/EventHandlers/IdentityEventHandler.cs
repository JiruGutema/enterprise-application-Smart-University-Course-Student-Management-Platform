using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;

namespace SmartUniversity.Modules.Enrollment.Application.EventHandlers
{
    public class IdentityEventHandler
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityEventHandler(IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleUserAccountDeactivatedAsync(UserAccountDeactivatedEvent evt)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(evt.UserId, CancellationToken.None);
        foreach (var enrollment in enrollments)
        {
            enrollment.Drop();
        }

        await _unitOfWork.CommitAsync(CancellationToken.None);
    }
}

}
