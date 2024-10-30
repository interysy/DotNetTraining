using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Services
{

    public interface IAuthorService
    {
        public abstract Author? GetRawAuthorById(long authorId);
    }

    public class AuthorService : IAuthorService
    {
        LibraryManagementContext _context;
        public AuthorService(LibraryManagementContext context)  
        {
            _context = context;
        }

        public Author? GetRawAuthorById(long authorId) => _context.Authors.Find(authorId);
    }
}
