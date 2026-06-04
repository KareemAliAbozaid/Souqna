

namespace Souqna.Domin.Entities
{
    public class Product: BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int CategoryId { get; set; }
        // The Id of the seller (Identity User Id) who owns this product
        public string? SellerId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
