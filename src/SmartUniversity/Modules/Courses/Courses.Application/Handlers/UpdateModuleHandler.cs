using MediatR;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class UpdateModuleHandler : IRequestHandler<UpdateModuleCommand, Unit>
{
    private readonly IModuleRepository _moduleRepository;

    public UpdateModuleHandler(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<Unit> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        module.Update(request.Name, request.Description, request.Order);
        await _moduleRepository.UpdateAsync(module);

        return Unit.Value;
    }
}