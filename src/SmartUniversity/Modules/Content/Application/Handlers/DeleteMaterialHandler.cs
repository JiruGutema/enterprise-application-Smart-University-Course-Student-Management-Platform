using MediatR;
using SmartUniversity.Modules.Content.Application.Commands;
using SmartUniversity.Modules.Content.Domain.Repositories;

namespace SmartUniversity.Modules.Content.Application.Handlers;

public class DeleteMaterialHandler : IRequestHandler<DeleteMaterialCommand, bool>
{
    private readonly IMaterialRepository _repository;

    public DeleteMaterialHandler(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _repository.GetByIdAsync(request.MaterialId);
        
        if (material == null)
            return false;

        // Delete physical file if it exists
        if (File.Exists(material.FilePath))
        {
            File.Delete(material.FilePath);
        }

        // Delete from database
        await _repository.DeleteAsync(material);
        
        return true;
    }
}