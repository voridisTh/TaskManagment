using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace TaskManagmentClient.Pages
{
    public partial class ProjectsBase : ComponentBase
    {
        [Inject]
        protected HttpClient Http { get; set; }

        protected List<ProjectDto>? projects;

        protected ProjectDto newProject = new ProjectDto();

        protected List<StatusDto> statuses = new();

        protected List<UserDto> users = new();

        protected string? errorMessage;

        protected List<TaskDto>? projectTasks;

        protected int selectedProjectId;

        protected CreateTaskDto newTask = new CreateTaskDto();

        protected ProjectDto? editingProject;

        protected TaskDto? editingTask;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                projects = await Http.GetFromJsonAsync<List<ProjectDto>>("api/TicketManagment/projects");
                statuses = await Http.GetFromJsonAsync<List<StatusDto>>("api/TicketManagment/statuses");
                users = await Http.GetFromJsonAsync<List<UserDto>>("api/TicketManagment/users");
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading data: {ex.Message}";
            }
        }

        protected async Task AddProjectAsync()
        {
            errorMessage = null;

            var response = await Http.PostAsJsonAsync("api/TicketManagment/projects", newProject);

            if (response.IsSuccessStatusCode)
            {
                projects = await Http.GetFromJsonAsync<List<ProjectDto>>("api/TicketManagment/projects");
                newProject = new ProjectDto();
            }
            else
            {
                errorMessage = $"Error adding project: {response.StatusCode}";
            }
        }
        protected async Task LoadTasksForProject(int projectId)
        {
            try
            {
                selectedProjectId = projectId;
                projectTasks = await Http.GetFromJsonAsync<List<TaskDto>>($"api/TicketManagment/project/{projectId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }
        protected async Task AddTaskAsync()
        {
            newTask.ProjectId = selectedProjectId;

            var response = await Http.PostAsJsonAsync("api/TicketManagment/tasks", newTask);

            if (response.IsSuccessStatusCode)
            {
                projectTasks = await Http.GetFromJsonAsync<List<TaskDto>>($"api/TicketManagment/project/{selectedProjectId}");
                newTask = new CreateTaskDto();
            }
            else
            {
                Console.WriteLine($"Failed to add task: {response.StatusCode}");
            }
        }
        protected void EditProject(ProjectDto project)
        {
            editingProject = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StatusId = project.StatusId
            };
        }

        protected async Task SaveProjectAsync(int projectId)
        {
            if (editingProject == null) return;

            var updateDto = new UpdateProjectDto
            {
                Id = editingProject.Id,
                Name = editingProject.Name,
                Description = editingProject.Description,
                StatusId = editingProject.StatusId
            };

            var response = await Http.PutAsJsonAsync($"api/TicketManagment/projects/{updateDto.Id}", updateDto);

            if (response.IsSuccessStatusCode)
            {
                projects = await Http.GetFromJsonAsync<List<ProjectDto>>("api/TicketManagment/projects");
                editingProject = null;
            }
        }

        protected void EditTask(TaskDto task)
        {
            editingTask = new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StatusId = task.StatusId,
                UserId = task.UserId,
                ProjectId = task.ProjectId
            };
        }

        protected async Task SaveTaskAsync(int taskId)
        {
            if (editingTask == null) return;

            var updateDto = new UpdateTaskDto
            {
                Id = editingTask.Id,
                Name = editingTask.Name,
                Description = editingTask.Description,
                StatusId = editingTask.StatusId,
                UserId = editingTask.UserId
            };

            var response = await Http.PutAsJsonAsync($"api/TicketManagment/tasks/{updateDto.Id}", updateDto);

            if (response.IsSuccessStatusCode)
            {
                await LoadTasksForProject(selectedProjectId);
                editingTask = null;
            }
        }
    }
}

