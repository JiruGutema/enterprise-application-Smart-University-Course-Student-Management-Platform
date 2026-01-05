using SmartUniversity.Modules.Content.Domain.Entities;
using SmartUniversity.Modules.Content.Domain.Repositories;

namespace SmartUniversity.Modules.Content.Application.Services;

public class MaterialService
{
    private readonly IMaterialRepository _repository;

    public MaterialService(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<Material> UploadAsync(
        Guid courseId,
        Guid? lessonId,
        string title,
        string fileName,
        string filePath,
        string fileType,
        long sizeInBytes,
        Guid uploadedByUserId,
        string? description)
    {
        var material = new Material(
            courseId,
            lessonId,
            title,
            fileName,
            filePath,
            fileType,
            sizeInBytes,
            uploadedByUserId,
            description
        );

        await _repository.AddAsync(material);
        return material;
    }
}
