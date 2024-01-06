using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskwety_Dotnet.Model;

namespace Taskwety_Dotnet.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskwetyDbContext _context;

        public TaskController(TaskwetyDbContext context)
        {
            _context = context;
        }

        // GET : api/Task/get-tasks
        [HttpGet("get-tasks")]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksForLoggedInUser()
        {
            // Get the user ID of the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_context.TaskModel == null)
            {
                return NotFound();
            }

            // Retrieve tasks based on the user ID
            var tasks = await _context.TaskModel
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return tasks;
        }

        // GET: api/Task/get-task/{id}
        [HttpGet("get-task/{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskModel(int id)
        {
            if (_context.TaskModel == null)
            {
                return NotFound();
            }

            var taskModel = await _context.TaskModel.FindAsync(id);

            if (taskModel == null)
            {
                return NotFound();
            }

            // Get the user ID of the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the task belongs to the authenticated user
            if (taskModel.UserId != userId)
            {
                // If not, return a 403 Forbidden response
                return Forbid();
            }

            return taskModel;
        }

        // POST: api/Task/create-task
        [HttpPost("create-task")]
        public async Task<ActionResult<TaskModel>> PostTaskModel(TaskModel taskModel)
        {
            if (_context.TaskModel == null)
            {
                return Problem("Entity set 'TaskwetyDbContext.TaskModel' is null.");
            }

            // Get the user ID of the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Set the user ID of the task to the ID of the currently authenticated user
            taskModel.UserId = userId;

            _context.TaskModel.Add(taskModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskModel", new { id = taskModel.Id }, taskModel);
        }

        // PUT: api/Task/edit-task/{id}
        [HttpPut("edit-task/{id}")]
        public async Task<IActionResult> PutTaskModel(int id, TaskModel taskModel)
        {

            if (_context.TaskModel == null)
            {
                return NotFound();
            }

            // Get the existing task based on the task ID
            var existingTask = await _context.TaskModel.FindAsync(id);

            if (existingTask == null)
            {
                return NotFound();
            }

            // Get the user ID of the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the task belongs to the authenticated user
            if (existingTask.UserId != userId)
            {
                return Forbid();
            }

            // Task belongs to the user, proceed with updating
            existingTask.TaskName = taskModel.TaskName;
            existingTask.TaskDescription = taskModel.TaskDescription;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskModelExists(id))
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


        // DELETE: api/Task/delete-task/{id}
        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> DeleteTaskModel(int id)
        {
            if (_context.TaskModel == null)
            {
                return NotFound();
            }

            // Get the task based on the task ID
            var taskModel = await _context.TaskModel.FindAsync(id);

            if (taskModel == null)
            {
                return NotFound();
            }

            // Get the user ID of the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the task belongs to the authenticated user
            if (taskModel.UserId != userId)
            {
                // If not, return a 403 Forbidden response
                return Forbid();
            }

            // Task belongs to the user, proceed with deletion
            _context.TaskModel.Remove(taskModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskModelExists(int id)
        {
            return (_context.TaskModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
