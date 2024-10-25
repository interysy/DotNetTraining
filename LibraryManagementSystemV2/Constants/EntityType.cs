namespace LibraryManagementSystemV2.Constants
{
    public static class EntityType
    {
        public const int Author = 0;
        public const int Renter = 1;
        public const int None = 2;


        public static readonly Dictionary<int, string> Types = new Dictionary<int, string>
        {
            { 0, "Author" },
            { 1, "Renter" }
        };
    }
} 

