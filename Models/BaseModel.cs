using System.ComponentModel.DataAnnotations;

namespace Caker.Models
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}
