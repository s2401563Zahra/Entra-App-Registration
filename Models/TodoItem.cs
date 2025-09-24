using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
	public class TodoItem
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string? Title { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		public bool IsCompleted { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? CompletedAt { get; set; }

		[Required]
		[MaxLength(100)]
		public string? UserId { get; set; } // This will store the user's object ID from Entra ID
	}
}