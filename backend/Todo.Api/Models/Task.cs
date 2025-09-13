using System.ComponentModel.DataAnnotations;

namespace Todo.Api.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(140)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public bool Completed { get; set; } = false;
        public DateTime? DueDate { get; set; }
        public ICollection<string> Tags { get; set; } = [];
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
