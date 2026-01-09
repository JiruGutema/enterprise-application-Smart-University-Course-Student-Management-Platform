using MediatR;

namespace SmartUniversity.Modules.Content.Application.Commands;

public class DeleteMaterialCommand : IRequest<bool>
{
    public Guid MaterialId { get; set; }
}