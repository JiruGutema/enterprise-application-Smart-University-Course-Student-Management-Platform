using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using SmartUniversity.Modules.Enrollment.Domain.Aggregates;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Application.Exceptions;


namespace SmartUniversity.Modules.Enrollment.Application.Commands
{
    public class EnrollStudentHandler : MediatR.IRequestHandler<EnrollStudentCommand, Guid>
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollStudentHandler(
            SmartUniversity.Modules.Enrollment.Domain.Repositories.IEnrollmentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(EnrollStudentCommand request, CancellationToken ct)
        {
            // Check if the student is already enrolled
            var alreadyEnrolled = await _repository.ExistsAsync(request.StudentId, request.CourseId, ct);
            if (alreadyEnrolled)
                throw new EnrollmentAlreadyExistsException();

            // Create enrollment entity
            var enrollment = new SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment(
                request.StudentId,
                request.CourseId
            );

            // Save enrollment
            await _repository.AddAsync(enrollment, ct);
            await _unitOfWork.CommitAsync(ct);

            return enrollment.Id;
        }
    }
}
