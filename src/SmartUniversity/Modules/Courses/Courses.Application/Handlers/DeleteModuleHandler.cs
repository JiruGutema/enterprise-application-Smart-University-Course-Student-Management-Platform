using MediatR;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class DeleteModuleHandler : IRequestHandler<DeleteModuleCommand, Unit>
{
    private readonly IModuleRepository _moduleRepository;

    public DeleteModuleHandler(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<Unit> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        await _moduleRepository.DeleteAsync(module);

        return Unit.Value;
    }
}