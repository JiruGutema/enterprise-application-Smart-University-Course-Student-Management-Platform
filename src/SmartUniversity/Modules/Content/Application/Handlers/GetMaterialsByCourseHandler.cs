using MediatR;
using SmartUniversity.Modules.Content.Application.Queries;
using SmartUniversity.Modules.Content.API.DTOs;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Domain.Aggregates;

namespace SmartUniversity.Modules.Content.Application.Handlers;

public class GetMaterialsByCourseHandler : IRequestHandler<GetMaterialsByCourseQuery, PagedResult<MaterialDto>>
{
    private readonly IMaterialRepository _repository;

    public GetMaterialsByCourseHandler(IMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MaterialDto>> Handle(GetMaterialsByCourseQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetByCourseIdAsync(
            request.CourseId,
            request.LessonId,
            request.FileType,
            request.Search,
            request.Sort,
            request.Page,
            request.PageSize
        );

        return new PagedResult<MaterialDto>
        {
            Data = items.Select(MapToDto),
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
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