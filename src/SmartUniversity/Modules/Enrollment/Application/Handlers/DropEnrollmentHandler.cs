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

            if (enrollment.Status == EnrollmentStatus.Dropped)
                return Unit.Value; // idempotent

            enrollment.Drop(); // domain method

            await _unitOfWork.CommitAsync(ct);

            return Unit.Value;
        }
    }
}
