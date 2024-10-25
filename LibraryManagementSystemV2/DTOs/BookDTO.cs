using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs
{
    public class BookDTO
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; } 
        public required AuthorBook[] Authors { get; set; }
    }
}
