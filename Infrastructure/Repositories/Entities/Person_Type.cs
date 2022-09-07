using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Person_Type : BaseEntity
    {  
        [Required]
        public int TypeId { get; set; }
        [Required]
        [ForeignKey("TypeId")]
        public Type Type { get; set; }
        [Required]
        public int PersonId { get; set; }
        [Required]
        [ForeignKey("PersonId")]
        public Person Person { get; set; }
    }

}
