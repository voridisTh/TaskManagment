using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using static TaskManagmentAPI.ServiceInterfaces.ITicketManagmentService;
using static TaskManagmentAPI.Service.TicketManagmentService;
using TaskManagmentAPI.ServiceInterfaces;
using Shared.DTOs;

namespace TaskManagmentAPI.ServiceControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketManagmentController : ControllerBase
    {
        private readonly ITicketManagmentService _ticketService;

        public TicketManagmentController(ITicketManagmentService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var user = await _ticketService.AuthenticateAsync(login.Username, login.Password);
                if (user == null)
                    return Unauthorized("Invalid username or password.");

                return Ok(new UserDto { Id = user.Id, Username = user.Username }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _ticketService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpPost("projects")]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdProject = await _ticketService.AddProjectAsync(dto);
                return CreatedAtAction(nameof(GetAllProjects), new { id = createdProject.Id }, createdProject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _ticketService.GetAllStatusesAsync();
            return Ok(statuses);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProjectId(int projectId)
        {
            var tasks = await _ticketService.GetTasksByProjectIdAsync(projectId);
            return Ok(tasks);
        }

        [HttpPost("tasks")]
        public async Task<ActionResult<TaskDto>> AddTask([FromBody] CreateTaskDto createTaskDto)
        {
            var task = await _ticketService.AddTaskAsync(createTaskDto);
            return Ok(task);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _ticketService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("projects/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updatedProject)
        {
            if (id != updatedProject.Id)
                return BadRequest("Project ID mismatch");

            try
            {
                var project = await _ticketService.UpdateProjectAsync(updatedProject);
                return Ok(project);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("tasks/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updatedTask)
        {
            if (id != updatedTask.Id)
                return BadRequest("Task ID mismatch");

            try
            {
                var task = await _ticketService.UpdateTaskAsync(updatedTask);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}