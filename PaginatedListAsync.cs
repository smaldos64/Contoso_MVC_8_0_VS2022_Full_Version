namespace Contoso_MVC_8_0_VS2022
{
  public class PaginatedListAsync<T> : List<T>
  {
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedListAsync(List<T> items, int count, int pageIndex, int pageSize)
    {
      PageIndex = pageIndex;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);

      this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedListAsync<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
      try
      {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedListAsync<T>(items, count, pageIndex, pageSize);
      }
      catch (Exception ex)
      {
        string ErrorString = ex.ToString();
        return null;
      }
    }
  }
}
