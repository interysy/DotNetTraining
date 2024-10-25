namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RentalCreateWithDateDTO
    {
        public required long BookId { get; set; }
        public required long RenterId { get; set; }
        public required DateTime ReturnDate { get; set; }
    }
}
