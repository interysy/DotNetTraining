using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystemV2.Models
{
    public class Renter
    {
        [Key]
        public long Id { get; set; }
        public virtual required Entity Entity { get; set; }
    }
}
