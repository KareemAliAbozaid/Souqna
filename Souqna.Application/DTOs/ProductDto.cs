using Microsoft.AspNetCore.Http;

namespace Souqna.Application.DTOs
{
    public record ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<PhotoDto> Photos { get; set; }
    }

    public record PhotoDto
    {
        public string ImageName { get; set; }
        public int ProductId { get; set; }
    }

    public record AddProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int CategoryId { get; set; }
        public IFormFileCollection Photos { get; set; }
    }

    public record UpdateProductDto : AddProductDto
    {
        public int Id { get; set; }
    }
}
