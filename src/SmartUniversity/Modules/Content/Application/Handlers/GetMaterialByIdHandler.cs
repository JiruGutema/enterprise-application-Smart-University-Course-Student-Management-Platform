using MediatR;
using SmartUniversity.Modules.Content.Application.Queries;
using SmartUniversity.Modules.Content.API.DTOs;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Domain.Aggregates;

namespace SmartUniversity.Modules.Content.Application.Handlers;

public class GetMaterialByIdHandler : IRequestHandler<GetMaterialByIdQuery, MaterialDto?>
{
    private readonly IMaterialRepository _repository;

    public GetMaterialByIdHandler(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaterialDto?> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var material = await _repository.GetByIdAsync(request.MaterialId);
        
        if (material == null)
            return null;

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