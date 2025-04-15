using System.ComponentModel.DataAnnotations.Schema;

namespace Caker.Models
{
    public class Message : BaseModel
    {
        public required int FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual User? From { get; set; }
        public required int ToId { get; set; }

        [ForeignKey("ToId")]
        public virtual User? To { get; set; }
        public string? Image { get; set; }
        public string? Text { get; set; }
        public required DateTime Date { get; set; }
    }
}
