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
    public class ManagmentBase : ComponentBase
    {
        [Inject] public NavigationManager Nav { get; set; } = default!;
        [Inject] protected HttpClient Http { get; set; }
        protected List<ProjectDto> Projects { get; set; } = new();

        protected List<StatusDto> statuses = new();
        protected bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;
            Projects = await Http.GetFromJsonAsync<List<ProjectDto>>("api/TicketManagment/projects");
            statuses = await Http.GetFromJsonAsync<List<StatusDto>>("api/TicketManagment/statuses");
            IsBusy = false;

        }

        protected void NavToEdit(int id)
        {
            Nav.NavigateTo($"/managment/edit/{id}");
        }
        protected void OpenTasks(int projectId)
        {
            Nav.NavigateTo($"/managment/tasks/{projectId}");
        }
    }
}
