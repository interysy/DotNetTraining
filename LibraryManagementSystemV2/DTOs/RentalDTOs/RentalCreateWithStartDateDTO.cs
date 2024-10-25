namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RentalCreateWithStartDateDTO : RentalCreateWithDateDTO
    {
        public required DateTime RentalDate { get; set; }
    }
}
