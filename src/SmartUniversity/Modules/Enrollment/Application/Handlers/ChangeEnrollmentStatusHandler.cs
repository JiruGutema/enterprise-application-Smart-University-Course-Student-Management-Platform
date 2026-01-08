using MediatR;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Domain.Aggregates;
using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.Enrollment.Domain.Enums;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Enrollment.Application.Handlers
{
    public class ChangeEnrollmentStatusHandler : IRequestHandler<ChangeEnrollmentStatusCommand>
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeEnrollmentStatusHandler(IEnrollmentRepository repository,
        IUnitOfWork unitOfWork)
        {
            _repository = repository;
           _unitOfWork = unitOfWork;

        }

       public async Task<Unit> Handle(ChangeEnrollmentStatusCommand request, CancellationToken ct)
{
    var enrollment = await _repository.GetAsync(request.EnrollmentId, ct)
        ?? throw new Exception("Enrollment not found");

    if (!Enum.TryParse<EnrollmentStatus>(request.Status, true, out var newStatus))
        throw new Exception("Invalid status");

    enrollment.ChangeStatus(newStatus);

    await _unitOfWork.CommitAsync(ct);

    return Unit.Value;
}

    }
}
