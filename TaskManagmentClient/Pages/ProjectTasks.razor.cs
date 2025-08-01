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
    public class ProjectTasksBase : ComponentBase
    {
        [Parameter] public int ProjectId { get; set; }

        protected List<TaskDto> Tasks = new();
        protected bool IsLoaded = false;
        protected string? ErrorMessage;
        protected string ProjectName { get; set; } = string.Empty;

        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Tasks = await Http.GetFromJsonAsync<List<TaskDto>>($"api/TicketManagment/project/{ProjectId}");
                var project = await Http.GetFromJsonAsync<ProjectDto>($"api/TicketManagment/projects/{ProjectId}");
                ProjectName = project?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading tasks: {ex.Message}";
            }

            IsLoaded = true;
        }

        protected void AddNewTask() => Nav.NavigateTo($"/managment/tasks/edit/{ProjectId}/0");

        protected void EditTask(int taskId) => Nav.NavigateTo($"/managment/tasks/edit/{ProjectId}/{taskId}");
        protected void GoBack() => Nav.NavigateTo("/managment");
    }
}