namespace api.Helpers
{
    public class PaginationParams
    {
        private const int MAxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize { get; set; } = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAxPageSize) ? MAxPageSize : value;
        }

    }
}
