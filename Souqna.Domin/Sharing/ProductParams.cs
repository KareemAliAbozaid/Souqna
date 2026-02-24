namespace Souqna.Domin.Sharing
{
    public class ProductParams
    {
        public string? Sort { get; set; }
        public int? CategoryId { get; set; }
        public string? Search { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 1 : (value > MaxPageSize ? MaxPageSize : value);
        }

        public int PageNumber { get; set; } = 1;

        public int MaxPageSize { get; set; } = 10;
    }
}