using MediatR;
using SmartUniversity.Modules.Content.Application.Commands;
using SmartUniversity.Modules.Content.API.DTOs;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Domain.Aggregates;

namespace SmartUniversity.Modules.Content.Application.Handlers;

public class UploadMaterialHandler : IRequestHandler<UploadMaterialCommand, MaterialDto>
{
    private readonly IMaterialRepository _repository;

    public UploadMaterialHandler(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaterialDto> Handle(UploadMaterialCommand request, CancellationToken cancellationToken)
    {
        // Create upload directory
        var path = Path.Combine("uploads", request.CourseId.ToString());
        Directory.CreateDirectory(path);
        var fullPath = Path.Combine(path, request.File.FileName);

        // Save file
        using (var stream = File.Create(fullPath))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Create material entity
        var material = new Material(
            request.CourseId,
            request.LessonId,
            request.Title ?? request.File.FileName,
            request.File.FileName,
            fullPath,
            request.File.ContentType,
            request.File.Length,
            request.UploadedByUserId,
            request.Description
        );

        // Save to database
        var savedMaterial = await _repository.AddAsync(material);

        // Return DTO
        return MapToDto(savedMaterial);
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