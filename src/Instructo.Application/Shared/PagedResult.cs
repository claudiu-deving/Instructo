namespace Application.Shared;

public record PagedResult<T>(int TotalCount, int PageNumber, int PageSize)
{
    public IEnumerable<T> Items { get; set; } = [];

    public int TotalPages => (int)Math.Ceiling(TotalCount/(double)PageSize);
    public bool HasPrevious => PageNumber>1;
    public bool HasNext => PageNumber<TotalPages;
}
