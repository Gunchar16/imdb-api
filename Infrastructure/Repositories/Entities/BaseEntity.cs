using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.Entities
{
    public interface IPrimaryKey
    {
        int Id { get; set; }
    }

    public abstract class BaseEntity : IPrimaryKey
    {
        [Key]
        public virtual int Id { get; set; }
    }
}
