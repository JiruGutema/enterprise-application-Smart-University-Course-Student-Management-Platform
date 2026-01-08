using MediatR;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Enrollment.Application.Handlers
{
    public class DropEnrollmentHandler : IRequestHandler<DropEnrollmentCommand>
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DropEnrollmentHandler(
            IEnrollmentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

       public async Task<Unit> Handle(DropEnrollmentCommand request, CancellationToken ct)
{
    var enrollment = await _repository.GetAsync(request.EnrollmentId, ct);

    if (enrollment == null)
        throw new KeyNotFoundException("Enrollment not found.");

    if (!request.IsAdmin && enrollment.StudentId != request.ActorUserId)
        throw new UnauthorizedAccessException("You cannot drop this enrollment.");

    if (enrollment.Status == EnrollmentStatus.Dropped)
        return Unit.Value; 
    enrollment.Drop();

    await _unitOfWork.CommitAsync(ct);

    return Unit.Value;
}

    }
}
