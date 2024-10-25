using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystemV2.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 
        public long BookId { get; set; } 
        public long RenterId { get; set; }
        public virtual required Book Book { get; set; }
        public virtual required Renter Renter { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public bool returned { get; set; } = false;
    }
}
