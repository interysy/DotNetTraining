namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RentalCreateWithDaysDTO
    {
        public required long BookId { get; set; }
        public required long RenterId { get; set; }
        public virtual long Days { get; set; }
    }
}
