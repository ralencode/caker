using System.ComponentModel.DataAnnotations;

namespace Caker.Models
{
    public class BaseModel
    {
        [Key]
        public int? Id { get; set; }
    }
}
