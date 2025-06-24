using DataAccess.Entities;
using Shared.DTOs;

namespace TaskManagmentAPI.ServiceInterfaces
{
        public interface ITicketManagmentService
        {
           Task<User?> AuthenticateAsync(string username, string password);
           Task<List<ProjectDto>> GetAllProjectsAsync();
           Task<ProjectDto> AddProjectAsync(ProjectDto projectDto);
           Task<List<StatusDto>> GetAllStatusesAsync();
           Task<List<TaskDto>> GetTasksByProjectIdAsync(int projectId);
           Task<TaskDto> AddTaskAsync(CreateTaskDto createTaskDto);
           Task<List<UserDto>> GetAllUsersAsync();
           Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updatedProject);
           Task<TaskDto> UpdateTaskAsync(UpdateTaskDto updatedTask);
        }

}

