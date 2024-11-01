using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;


namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class BookUtility : IDataUtility<Book>
    {
        internal static readonly List<Book> testBooks = new List<Book>
                        {
                                new Book
                                { 
                                    Id = 1,
                                    Name = "The Great Gatsby",
                                    Description = "A novel written by American author F. Scott Fitzgerald.",
                                    Price = 10.99f,
                                    Quantity = 5
                                },
                                new Book
                                { 
                                    Id = 2,
                                    Name = "1984",
                                    Description = "A dystopian social science fiction novel and cautionary tale by the English writer George Orwell.",
                                    Price = 8.99f,
                                    Quantity = 10
                                },
                                new Book
                                { 
                                    Id = 3,
                                    Name = "To Kill a Mockingbird",
                                    Description = "A novel by Harper Lee published in 1960.",
                                    Price = 12.99f,
                                    Quantity = 7
                                },
                                new Book
                                { 
                                    Id = 4, 
                                    Name = "Pride and Prejudice",
                                    Description = "A romantic novel of manners written by Jane Austen.",
                                    Price = 9.99f,
                                    Quantity = 3
                                },
                                new Book
                                { 
                                    Id = 5,
                                    Name = "Good Omens",
                                    Description = "A novel by Terry Pratchett and Neil Gaiman.",
                                    Price = 11.99f,
                                    Quantity = 2
                                }
                            }; 


        public static async Task<Book?> GetAdded(LibraryManagementContext context, long id)
        {
             var book = await context.Books.FindAsync(id);
             return book;
            
        }

        public async Task SeedData(LibraryManagementContext context)
        {
                context.Books.AddRange(testBooks);
                await context.SaveChangesAsync();
        }

        public BookCreateDTO NewEmptyBookToCreate()
        {
            return new BookCreateDTO
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
                AuthorIDs = new List<long> { },
                NewAuthors = new List<AuthorCreateDTO> { }
            };
        }

        public BookCreateDTO NewBookToCreate(ICollection<long> existingAuthors)
        {

            var newBook = NewEmptyBookToCreate();
            newBook.AuthorIDs = existingAuthors;
            return newBook;
        }

        public BookCreateDTO NewBookToCreate(ICollection<AuthorCreateDTO> newAuthors)
        {
            var newBook = NewEmptyBookToCreate();
            newBook.NewAuthors = newAuthors;
            return newBook;
        }
    }
}
