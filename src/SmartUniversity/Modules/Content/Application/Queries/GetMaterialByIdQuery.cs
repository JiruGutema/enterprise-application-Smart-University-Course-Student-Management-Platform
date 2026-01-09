using MediatR;
using SmartUniversity.Modules.Content.API.DTOs;

namespace SmartUniversity.Modules.Content.Application.Queries;

public class GetMaterialByIdQuery : IRequest<MaterialDto?>
{
    public Guid MaterialId { get; set; }
}