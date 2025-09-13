using System.ComponentModel.DataAnnotations;

namespace Todo.Api.DTOs
{
    public static class TaskDtos
    {
        public class Create
        {
            [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required and must not be empty.")]
            [StringLength(140, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 140 characters.")]
            public string Title { get; set; } = string.Empty;
            public DateTime? DueDate { get; set; }
            public ICollection<string> Tags { get; set; } = [];
        }

        public class Update
        {
            [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
            [StringLength(140, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 140 characters.")]
            public string Title { get; set; } = string.Empty;
            [Required]
            public bool Completed { get; set; }
            public DateTime? DueDate { get; set; }
            public ICollection<string> Tags { get; set; } = [];
        }

        public class Response
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public bool Completed { get; set; }
            public DateTime? DueDate { get; set; }
            public ICollection<string> Tags { get; set; } = [];
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
