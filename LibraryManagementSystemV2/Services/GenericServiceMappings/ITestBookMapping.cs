using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Services.GenericServiceMappings
{
    public interface ITestBookMapping : IGenericService<Book, BookShowDTO, BookCreateDTO, BookUpdateDTO>
    {
    }
}
