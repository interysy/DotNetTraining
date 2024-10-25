using LibraryManagementSystemV2.DTOs.EntityDTOs;

namespace LibraryManagementSystemV2.DTOs.RenterDTOs
{
    public class RenterCreateDTO 
    {
        public virtual required EntityCreateDTO Entity { get; set; }
    }
}
