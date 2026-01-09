using MediatR;
using SmartUniversity.Modules.Content.Application.Commands;
using SmartUniversity.Modules.Content.API.DTOs;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Domain.Aggregates;

namespace SmartUniversity.Modules.Content.Application.Handlers;

public class UpdateMaterialHandler : IRequestHandler<UpdateMaterialCommand, MaterialDto?>
{
    private readonly IMaterialRepository _repository;

    public UpdateMaterialHandler(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaterialDto?> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _repository.GetByIdAsync(request.MaterialId);
        
        if (material == null)
            return null;

        material.UpdateMetadata(request.Title, request.Description, request.LessonId);
        await _repository.UpdateAsync(material);

        return MapToDto(material);
    }

    private static MaterialDto MapToDto(Material material) => new()
    {
        MaterialId = material.Id,
        CourseId = material.CourseId,
        LessonId = material.LessonId,
        Title = material.Title,
        FileName = material.FileName,
        FilePath = material.FilePath,
        FileType = material.FileType,
        SizeInBytes = material.SizeInBytes,
        UploadDate = material.UploadDate,
        UploadedByUserId = material.UploadedByUserId,
        Description = material.Description
    };
}