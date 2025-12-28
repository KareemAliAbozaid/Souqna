

namespace Souqna.Domin.DTOs
{
    public record CategoryDto(string Name,string? Description);
    public record UpdateCategoryDto(int Id,string Name, string? Description);

}
