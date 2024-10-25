﻿using AutoMapper;
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

        public async Task<BookShowDTO> UpdateAsync(long id, BookUpdateDTO dto)
        {

            if (!_unitOfWork.Repository<Book>().Exists(book => book.Id == id))  
            {
                throw new BookNotFoundException($"Book with id of {id} cannot be found.");
            }

            var transaction = await _unitOfWork.StartTransactionAsync();

            using (transaction)
            {
                try
                {
                    Book bookUpdated = BookUpdateDTO.BookUpdateDTOToBook(dto, id);
                    _unitOfWork.Repository<Book>().SetEntryStateToModified(bookUpdated);

                    IEnumerable<AuthorBook> bookAuthors = await _unitOfWork.Repository<AuthorBook>().GetAllAsync(authorBook => authorBook.BookId == id,
                                                                                                                 true,
                                                                                                                 authorBook => authorBook.Author
                                                                                                                 );

                    IEnumerable<AuthorBook> bookAuthorsToRemove = bookAuthors.Where(authorBook => !dto.AuthorIDs.Contains(authorBook.AuthorId));
                    IEnumerable<long> bookAuthorIdsToAdd = dto.AuthorIDs.Where(authorId => !bookAuthors.Select(bookAuthor => bookAuthor.AuthorId).Contains(authorId));


                    _unitOfWork.Repository<AuthorBook>().DeleteRange(bookAuthorsToRemove); 



                    foreach (var authorId in bookAuthorIdsToAdd)
                    {
                        var author = await _unitOfWork.Repository<Author>().GetByIdAsync(authorId);  
                            
                        if (author == null)
                        {
                            throw new BookNotFoundException($"Author with ID {authorId} not found.");
                        }

                        AuthorBook authorBook = AuthorBook.AuthorAndBookToAuthorBook(author, bookUpdated);

                        await _unitOfWork.Repository<AuthorBook>().AddAsync(authorBook); 
                    }

                    if (dto.NewAuthors is null)
                    {
                        await _unitOfWork.SaveChangesAsync(); 
                        
                    }


                    _bookAuthorService.CreateAuthorsForBookFromCreateDTO(dto.NewAuthors, bookUpdated);



                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return await GetByIdAsync(bookUpdated.Id);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}