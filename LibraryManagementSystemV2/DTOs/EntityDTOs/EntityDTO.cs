using LibraryManagementSystemV2.Constants;

namespace LibraryManagementSystemV2.DTOs.EntityDTOs
{
    public class EntityDTO
    {
        public long Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
