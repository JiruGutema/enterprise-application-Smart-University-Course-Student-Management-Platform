namespace SmartUniversity.Modules.Content.Application.Exceptions;

public class MaterialNotFoundException : Exception
{
    public MaterialNotFoundException(Guid materialId) 
        : base($"Material with ID {materialId} was not found.")
    {
    }
}