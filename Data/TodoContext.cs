using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
	public class TodoContext : DbContext
	{
		public TodoContext(DbContextOptions<TodoContext> options) : base(options)
		{
		}

		public DbSet<TodoItem> TodoItems { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TodoItem>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
				entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

				// Create an index on UserId for better query performance
				entity.HasIndex(e => e.UserId).HasDatabaseName("IX_TodoItems_UserId");

				// Create an index on CreatedAt for better query performance
				entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_TodoItems_CreatedAt");
			});
		}
	}
}