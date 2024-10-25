using AutoMapper;
using LibraryManagementSystemV2.CustomExceptions.Books;
using LibraryManagementSystemV2.DTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Transactions;

namespace LibraryManagementSystemV2.Services
{
    public class TestBookService : GenericService<Book, BookShowDTO, BookCreateDTO, BookUpdateDTO>, ITestBookMapping
    {
        private readonly IBookAuthorService _bookAuthorService;

        public TestBookService(IUnitOfWork unitOfWork, IMapper mapper, IBookAuthorService bookAuthorsService) : base(unitOfWork, mapper)
        {
            _bookAuthorService = bookAuthorsService;
        }

        public new async Task<IEnumerable<BookShowDTO>> GetAllAsync()
        {
            List<Book?> books = await _unitOfWork.Repository<Book>().GetAllAsync();
            List<BookShowDTO> bookDTOs = new List<BookShowDTO>();

            foreach (Book? book in books)
            {
                if (book is null)
                {
                    continue;
                }

                IEnumerable<Author> bookAuthors = await _bookAuthorService.GetBookAuthors(book.Id);
                BookShowDTO bookShowDTO = BookShowDTO.BookToBookShowDTO(book, bookAuthors);
                bookDTOs.Add(bookShowDTO);
            }

            return bookDTOs;
        }

        public new async Task<BookShowDTO> GetByIdAsync(long id)
        {
            Book book = await _unitOfWork.Repository<Book>().GetByIdAsync(id);

            if (book is null)
            {
                throw new BookNotFoundException($"Book with ID {id} not found.");
            }

            IEnumerable<Author> bookAuthors = (await _bookAuthorService.GetBookAuthors(book.Id));
            BookShowDTO bookShowDTO = BookShowDTO.BookToBookShowDTO(book, bookAuthors);

            return bookShowDTO;
        }


        public new async Task<BookShowDTO> AddAsync(BookCreateDTO dto) { 

            var transaction = await _unitOfWork.StartTransactionAsync();

            using (transaction)
            {
                Book book = BookCreateDTO.BookCreateDTOToBook(dto); 
                await _unitOfWork.Repository<Book>().AddAsync(book);

                IEnumerable<Author> authorsToAdd = await _unitOfWork.Repository<Author>().GetAllAsync(author => dto.AuthorIDs.Contains(author.Id), true, author => author.Entity);

                foreach (var author in authorsToAdd)
                {
                    await _unitOfWork.Repository<AuthorBook>().AddAsync(AuthorBook.AuthorAndBookToAuthorBook(author, book));
                }

                foreach (var newAuthor in dto.NewAuthors)  
                {
                    Entity entity = Entity.CreateEntity(newAuthor.Entity.FirstName, newAuthor.Entity.LastName);
                    await _unitOfWork.Repository<Entity>().AddAsync(entity);

                    Author author = Author.CreateAuthorFromEntity(entity);
                    await _unitOfWork.Repository<Author>().AddAsync(author);

                    await _unitOfWork.Repository<AuthorBook>().AddAsync(AuthorBook.AuthorAndBookToAuthorBook(author, book));
                }

                await _unitOfWork.SaveChangesAsync(); 
                BookShowDTO resultantBook = await GetByIdAsync(book.Id);

                await transaction.CommitAsync();
                return resultantBook;
            }
        }


        //public new async Task UpdateAsync(long id, BookUpdateDTO dto) {

        //    Book? book = await _unitOfWork.Repository<Book>().GetByIdAsync(id); 

        //    if (book is null)
        //    {
        //        throw new BookNotFoundException($"Book with id of {id} cannot be found.");
        //    }

        //    var transaction = await _unitOfWork.StartTransactionAsync(); 

        //    using (transaction)
        //    {
        //        try
        //        {
        //            Book bookUpdated = BookUpdateDTO.BookUpdateDTOToBook(dto, id);
        //            _unitOfWork.Repository<Book>().Entry(bookUpdated).State = EntityState.Modified;


        //            IEnumerable<AuthorBook> bookAuthors = _context.AuthorBooks.Where(authorBook => authorBook.BookId == id).Include(authorBook => authorBook.Author).Include(authorBook => authorBook.Book);
        //            IEnumerable<AuthorBook> bookAuthorsToRemove = bookAuthors.Where(authorBook => !bookDTO.AuthorIDs.Contains(authorBook.AuthorId));
        //            IEnumerable<long> bookAuthorIdsToAdd = bookDTO.AuthorIDs.Where(authorId => !bookAuthors.Select(bookAuthor => bookAuthor.AuthorId).Contains(authorId));


        //            _context.AuthorBooks.RemoveRange(bookAuthorsToRemove);


        //            foreach (var authorId in bookAuthorIdsToAdd)
        //            {
        //                var author = _context.Authors.Find(authorId);
        //                if (author == null)
        //                {
        //                    return NotFound($"Author with ID {authorId} not found.");
        //                }

        //                AuthorBook authorBook = AuthorBook.AuthorAndBookToAuthorBook(author, bookUpdated);
        //                _context.AuthorBooks.Add(authorBook);
        //            }

        //            if (bookDTO.NewAuthors == null)
        //            {
        //                _context.SaveChanges();
        //                return NoContent();
        //            }


        //            _bookAuthorService.CreateAuthorsForBookFromCreateDTO(bookDTO.NewAuthors, bookUpdated);


        //            _context.SaveChanges();
        //            transaction.Commit();


        //            return NoContent();
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
    }
}
