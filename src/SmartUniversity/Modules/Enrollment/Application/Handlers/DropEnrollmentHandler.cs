using MediatR;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;

namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class DropEnrollmentHandler : IRequestHandler<DropEnrollmentCommand, Unit>
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
            var enrollment = await _repository.GetAsync(request.EnrollmentId, ct)
                ?? throw new InvalidOperationException("Enrollment not found");

            enrollment.Drop();
            await _unitOfWork.CommitAsync(ct);

            return Unit.Value; 
        }
    }
}
