using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystemV2.Models
{
    public class Author
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public virtual required Entity Entity { get; set; }

        public static Author CreateAuthorFromEntity(Entity entity) {

            return new Author
            {
                Entity = entity,
                Id = entity.Id
            };
        }
    }
}
