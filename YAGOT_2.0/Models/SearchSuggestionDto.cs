namespace YAGOT_2._0.Models;

public sealed class SearchSuggestionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}
