using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.Commands;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Handlers;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentResponse>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentResponse> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = new Assignment(
            request.CourseId,
            request.Title,
            request.Description,
            request.Type,
            request.DueDate,
            request.MaxScore,
            request.Weight
        );

        await _assignmentRepository.AddAsync(assignment);

        return new AssignmentResponse(
            assignment.AssignmentId,
            assignment.CourseId,
            assignment.Title,
            assignment.Description,
            assignment.Type,
            assignment.DueDate,
            assignment.MaxScore,
            assignment.Weight,
            assignment.CreatedAt
        );
    }
}

public class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand, AssignmentResponse>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public UpdateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentResponse> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId);
        if (assignment == null) throw new ArgumentException("Assignment not found");

        assignment.Update(request.Title, request.Description, request.DueDate, request.MaxScore, request.Weight);
        await _assignmentRepository.UpdateAsync(assignment);

        return new AssignmentResponse(
            assignment.AssignmentId,
            assignment.CourseId,
            assignment.Title,
            assignment.Description,
            assignment.Type,
            assignment.DueDate,
            assignment.MaxScore,
            assignment.Weight,
            assignment.CreatedAt
        );
    }
}

public class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public DeleteAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Unit> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        await _assignmentRepository.DeleteAsync(request.AssignmentId);
        return Unit.Value;
    }
}