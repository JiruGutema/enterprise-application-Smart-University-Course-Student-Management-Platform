using MediatR;
using SmartUniversity.Modules.Enrollment.Domain.Aggregates;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;

namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class EnrollStudentHandler : IRequestHandler<EnrollStudentCommand, Guid>
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollStudentHandler(
            IEnrollmentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(EnrollStudentCommand request, CancellationToken ct)
        {
            // Fully qualify the Enrollment class to avoid namespace conflict
            var enrollment = new SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment(
                request.StudentId,
                request.CourseId
            );

            await _repository.AddAsync(enrollment, ct);
            await _unitOfWork.CommitAsync(ct);

            return enrollment.Id;
        }
    }
}
