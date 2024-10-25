using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystemV2.Models
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public static Entity CreateEntity(string firstName, string lastName)  
        {
            return new Entity
            {
                FirstName = firstName,
                LastName = lastName
            };
        }

    }
}
