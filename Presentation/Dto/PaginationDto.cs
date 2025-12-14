namespace Presentation.Dto;

/// <summary>
/// Comprehensive pagination DTO containing data and metadata.
/// </summary>
public record PaginationDto<T>(
    IEnumerable<T> Data,
    int Page,
    int PageSize,
    int TotalCount
) where T : class
{
    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    
    /// <summary>
    /// Indicates if there is a previous page.
    /// </summary>
    public bool HasPrevious => Page > 1;
    
    /// <summary>
    /// Indicates if there is a next page.
    /// </summary>
    public bool HasNext => Page < TotalPages;
    
    /// <summary>
    /// The previous page number, or null if no previous page exists.
    /// </summary>
    public int? PreviousPage => HasPrevious ? Page - 1 : null;
    
    /// <summary>
    /// The next page number, or null if no next page exists.
    /// </summary>
    public int? NextPage => HasNext ? Page + 1 : null;
    
    /// <summary>
    /// Index of the first item on the current page (1-based).
    /// </summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;
    
    /// <summary>
    /// Index of the last item on the current page (1-based).
    /// </summary>
    public int LastItemIndex => Math.Min(Page * PageSize, TotalCount);
    
    /// <summary>
    /// Number of items in the current page.
    /// </summary>
    public int ItemCount => Data?.Count() ?? 0;
    
    /// <summary>
    /// Indicates if the current page is empty.
    /// </summary>
    public bool IsEmpty => ItemCount == 0;
    
    /// <summary>
    /// Indicates if this is the first page.
    /// </summary>
    public bool IsFirstPage => Page == 1;
    
    /// <summary>
    /// Indicates if this is the last page.
    /// </summary>
    public bool IsLastPage => Page >= TotalPages;
    
    /// <summary>
    /// Creates an empty pagination result.
    /// </summary>
    public static PaginationDto<T> Empty(int page = 1, int pageSize = 10) =>
        new([], page, pageSize, 0);
    
    /// <summary>
    /// Maps the data to a different type while preserving pagination metadata.
    /// </summary>
    public PaginationDto<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : class =>
        new(Data.Select(mapper),
            Page,
            PageSize,
            TotalCount);
}