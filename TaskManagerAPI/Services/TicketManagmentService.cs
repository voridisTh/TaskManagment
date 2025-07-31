using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using TaskManagmentAPI.ServiceInterfaces;
using static TaskManagmentAPI.Service.TicketManagmentService;
using static TaskManagmentAPI.ServiceInterfaces.ITicketManagmentService;

namespace TaskManagmentAPI.Service
{
    public class TicketManagmentService : ITicketManagmentService
    {
        private readonly AppDbContext _context;

        public TicketManagmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.ProjectStatus)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StatusName = p.ProjectStatus.Name
                })
                .ToListAsync();
        }
        public async Task<ProjectDto> AddProjectAsync(ProjectDto projectDto)
        {
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                StatusId = projectDto.StatusId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var statusName = await _context.Statuses
                .Where(s => s.Id == project.StatusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StatusId = project.StatusId,
                StatusName = statusName
            };
        }
        public async Task<List<StatusDto>> GetAllStatusesAsync()
        {
            return await _context.Statuses
                .Select(s => new StatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }
        public async Task<List<TaskDto>> GetTasksByProjectIdAsync(int projectId)
        {
            var tasks = await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    StatusId = t.StatusId,
                    StatusName = t.TaskStatus.Name,   
                    UserId = t.UserId,
                    UserName = t.TaskUser.Username  
                }).ToListAsync();

            return tasks;
        }
        public async Task<TaskDto> AddTaskAsync(CreateTaskDto newTask)
        {
            var taskEntity = new TaskItem
            {
                Name = newTask.Name,
                Description = newTask.Description,
                ProjectId = newTask.ProjectId,
                StatusId = newTask.StatusId,
                UserId = newTask.UserId ?? 0 
            };

            _context.TaskItems.Add(taskEntity);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = taskEntity.Id,
                Name = taskEntity.Name,
                Description = taskEntity.Description,
                ProjectId = taskEntity.ProjectId,
                StatusId = taskEntity.StatusId,
                StatusName = (await _context.Statuses.FindAsync(taskEntity.StatusId))?.Name,
                UserId = taskEntity.UserId,
                UserName = taskEntity.UserId == 0 ? null : (await _context.Users.FindAsync(taskEntity.UserId))?.Username
            };
        }
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username
                }).ToListAsync();
        }
        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updatedProject)
        {
            var project = await _context.Projects.FindAsync(updatedProject.Id);
            if (project == null)
                throw new Exception("Project not found");

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;
            project.StatusId = updatedProject.StatusId;

            await _context.SaveChangesAsync();

            var statusName = await _context.Statuses
                .Where(s => s.Id == project.StatusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StatusId = project.StatusId,
                StatusName = statusName
            };
        }
        public async Task<TaskDto> UpdateTaskAsync(UpdateTaskDto updatedTask)
        {
            var task = await _context.TaskItems.FindAsync(updatedTask.Id);
            if (task == null)
                throw new Exception("Task not found");

            task.Name = updatedTask.Name;
            task.Description = updatedTask.Description;
            task.StatusId = updatedTask.StatusId;
            task.UserId = updatedTask.UserId ?? 0;

            await _context.SaveChangesAsync();

            var statusName = await _context.Statuses
                .Where(s => s.Id == task.StatusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            var userName = (await _context.Users.FindAsync(task.UserId))?.Username;

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                ProjectId = task.ProjectId,
                StatusId = task.StatusId,
                StatusName = statusName,
                UserId = task.UserId,
                UserName = userName
            };
        }
        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectStatus)
                .Where(p => p.Id == id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StatusId = p.StatusId,
                    StatusName = p.ProjectStatus.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}