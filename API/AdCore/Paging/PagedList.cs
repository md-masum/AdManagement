namespace AdCore.Paging
{
    public class PagedList<TEntity>
    {
        public int TotalCount { get; set; }
        public IList<TEntity> Data { get; set; }

        public PagedList(List<TEntity> items, int count)
        {
            TotalCount = count;
            Data = items;
        }

        public static PagedList<TEntity> CreateAsync(IEnumerable<TEntity> source, int pageNumber, int pageSize, string searchKey = null)
        {
            var entities = source.ToList();
            var count = entities.Count;

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                var items = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<TEntity>(items, count);
            }
            else
            {
                var items = entities.Where(m => m != null && m.GetType().GetProperties().Any(x =>
                        x.GetValue(m, null) != null && x.GetValue(m, null)!.ToString()!.ToLower().Contains(searchKey.ToLower())))
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<TEntity>(items, count);
            }

        }
    }
}
