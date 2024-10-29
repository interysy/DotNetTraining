namespace LibraryManagementSystemV2.CustomExceptions.Renters
{
    public class RenterNotFoundException : Exception
    {
        public RenterNotFoundException()
        {
        }

        public RenterNotFoundException(string? message) : base(message)
        {
        }

        public RenterNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
