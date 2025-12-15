using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto;

public record PaginationRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    int Page = 1,
    
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    int PageSize = 10)
{
    /// <summary>
    /// Calculates the number of items to skip.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
    
    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    public int Take => PageSize;
}
