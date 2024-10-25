using AutoMapper;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.EntityDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using LibraryManagementSystemV2.Models;



namespace LibraryManagementSystemV2.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorCreateDTO>();
            CreateMap<Author, AuthorShowDTO>();
            CreateMap<Book, BookCreateDTO>();
            CreateMap<Book, BookShowDTO>();
            CreateMap<Book, BookUpdateDTO>();
            CreateMap<Entity, EntityCreateDTO>();
            CreateMap<Entity, EntityShowDTO>();
            CreateMap<Entity, EntitySupertypeDTO>();
            CreateMap<Entity, EntityCreateWithTypeDTO>();
            CreateMap<Renter, RenterCreateDTO>();
            CreateMap<Renter, RenterShowDTO>();
            CreateMap<Rental, RentalCreateWithDateDTO>(); 
            CreateMap<Rental, RentalShowDTO>();
            CreateMap<Rental, RentalCreateWithDaysDTO>();
            CreateMap<Rental, RentalCreateWithStartDateDTO>();
        }
    }
}
