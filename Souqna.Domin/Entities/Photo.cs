

namespace Souqna.Domin.Entities
{
    public class Photo: BaseEntity<int>
    {
        public string ImageName { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
