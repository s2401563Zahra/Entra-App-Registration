using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using System.Security.Claims;

namespace TodoApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize] // Requires authentication for all actions
	public class TodosController : ControllerBase
	{
		private readonly TodoContext _context;
		private readonly ILogger<TodosController> _logger;

		public TodosController(TodoContext context, ILogger<TodosController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/todos
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var todoItems = await _context.TodoItems
				.Where(t => t.UserId == userId)
				.OrderByDescending(t => t.CreatedAt)
				.ToListAsync();

			_logger.LogInformation("Retrieved {Count} todo items for user {UserId}", todoItems.Count, userId);
			return Ok(todoItems);
		}

		// GET: api/todos/5
		[HttpGet("{id}")]
		public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var todoItem = await _context.TodoItems
				.Where(t => t.Id == id && t.UserId == userId)
				.FirstOrDefaultAsync();

			if (todoItem == null)
			{
				return NotFound();
			}

			return Ok(todoItem);
		}

		// PUT: api/todos/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTodoItem(int id, TodoItem todoItem)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			if (id != todoItem.Id)
			{
				return BadRequest("ID mismatch");
			}

			var existingTodo = await _context.TodoItems
				.Where(t => t.Id == id && t.UserId == userId)
				.FirstOrDefaultAsync();

			if (existingTodo == null)
			{
				return NotFound();
			}

			// Update properties
			existingTodo.Title = todoItem.Title;
			existingTodo.Description = todoItem.Description;
			existingTodo.IsCompleted = todoItem.IsCompleted;

			if (todoItem.IsCompleted && existingTodo.CompletedAt == null)
			{
				existingTodo.CompletedAt = DateTime.UtcNow;
			}
			else if (!todoItem.IsCompleted)
			{
				existingTodo.CompletedAt = null;
			}

			try
			{
				await _context.SaveChangesAsync();
				_logger.LogInformation("Updated todo item {Id} for user {UserId}", id, userId);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TodoItemExists(id, userId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/todos
		[HttpPost]
		public async Task<ActionResult<TodoItem>> CreateTodoItem(CreateTodoRequest request)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var todoItem = new TodoItem
			{
				Title = request.Title,
				Description = request.Description,
				IsCompleted = request.IsCompleted,
				UserId = userId,
				CreatedAt = DateTime.UtcNow
			};

			_context.TodoItems.Add(todoItem);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Created new todo item {Id} for user {UserId}", todoItem.Id, userId);

			return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
		}

		// DELETE: api/todos/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTodoItem(int id)
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var todoItem = await _context.TodoItems
				.Where(t => t.Id == id && t.UserId == userId)
				.FirstOrDefaultAsync();

			if (todoItem == null)
			{
				return NotFound();
			}

			_context.TodoItems.Remove(todoItem);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Deleted todo item {Id} for user {UserId}", id, userId);

			return NoContent();
		}

		// GET: api/todos/completed
		[HttpGet("completed")]
		public async Task<ActionResult<IEnumerable<TodoItem>>> GetCompletedTodos()
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var completedTodos = await _context.TodoItems
				.Where(t => t.UserId == userId && t.IsCompleted)
				.OrderByDescending(t => t.CompletedAt)
				.ToListAsync();

			return Ok(completedTodos);
		}

		// GET: api/todos/pending
		[HttpGet("pending")]
		public async Task<ActionResult<IEnumerable<TodoItem>>> GetPendingTodos()
		{
			var userId = GetCurrentUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var pendingTodos = await _context.TodoItems
				.Where(t => t.UserId == userId && !t.IsCompleted)
				.OrderByDescending(t => t.CreatedAt)
				.ToListAsync();

			return Ok(pendingTodos);
		}

		private string? GetCurrentUserId()
		{
			// Get the object ID (oid) claim from the JWT token
			// This is the user's unique identifier in Azure AD
			return HttpContext.User?.FindFirst("oid")?.Value ??
				   HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		private bool TodoItemExists(int id, string userId)
		{
			return _context.TodoItems.Any(e => e.Id == id && e.UserId == userId);
		}
	}

	// DTO for creating todos
	public class CreateTodoRequest
	{
		public required string Title { get; set; }
		public string? Description { get; set; }
		public bool IsCompleted { get; set; } = false;
	}
}