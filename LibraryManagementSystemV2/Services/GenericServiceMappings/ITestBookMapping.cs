using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Services.Interfaces;

namespace LibraryManagementSystemV2.Services.GenericServiceMappings
{
    public interface ITestBookMapping : IGenericService<Book, BookShowDTO, BookCreateDTO, BookUpdateDTO>
    {
    }
}
