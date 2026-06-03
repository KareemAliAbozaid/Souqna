namespace Souqna.Application.DTOs
{
    public record CategoryDto(int Id,string Name, string? Description);
    public record UpdateCategoryDto(int Id, string Name, string? Description);
}
