using System.Runtime.Serialization;

namespace LibraryManagementSystemV2.CustomExceptions.Books
{
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException()
        {
        }

        public BookNotFoundException(string? message) : base(message)
        {
        }

        public BookNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
