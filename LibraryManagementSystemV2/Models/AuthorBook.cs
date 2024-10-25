using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.DTOs.NewFolder1;

namespace LibraryManagementSystemV2.Models
{
    [PrimaryKey(nameof(AuthorId), nameof(BookId))]
    public class AuthorBook
    {
        public long AuthorId { get; set; }

        public long BookId { get; set;} 

        public virtual required Author Author { get; set; }
        public virtual required Book Book { get; set; }

        public static AuthorBook AuthorAndBookToAuthorBook(Author author, Book book)
        {
            return new AuthorBook
            {
                AuthorId = author.Id,
                BookId = book.Id,
                Author = author,
                Book = book
            };
        }
    }
}
