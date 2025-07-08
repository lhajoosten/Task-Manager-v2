namespace TaskManager.Application.Common.Models;

public class PagedResult<T>
{
	public IEnumerable<T> Items { get; }
	public int TotalCount { get; }
	public int PageNumber { get; }
	public int PageSize { get; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
	public bool HasPrevious => PageNumber > 1;
	public bool HasNext => PageNumber < TotalPages;

	public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
	{
		Items = items;
		TotalCount = totalCount;
		PageNumber = pageNumber;
		PageSize = pageSize;
	}
}
