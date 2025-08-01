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
    public class EditTaskBase : ComponentBase
    {
        [Parameter] public int ProjectId { get; set; }
        [Parameter] public int TaskId { get; set; }

        protected TaskDto Model { get; set; } = new();
        protected List<StatusDto> Statuses = new();
        protected List<UserDto> Users = new();

        protected bool Busy { get; set; }
        protected string? Alert { get; set; }

        protected bool IsNew => TaskId == 0;

        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Statuses = await Http.GetFromJsonAsync<List<StatusDto>>("api/TicketManagment/statuses") ?? new();
            Users = await Http.GetFromJsonAsync<List<UserDto>>("api/TicketManagment/users") ?? new();

            if (!IsNew)
            {
                var task = await Http.GetFromJsonAsync<TaskDto>($"api/TicketManagment/tasks/{TaskId}");

                if (task != null)
                {
                    Model = task;
                }
                else
                {
                    Alert = "Task not found.";
                }
            }
            else
            {
                Model.ProjectId = ProjectId;
            }
        }

        protected async Task SaveAsync()
        {
            Busy = true;
            HttpResponseMessage resp;

            if (IsNew)
            {
                resp = await Http.PostAsJsonAsync("api/TicketManagment/tasks", Model);
                Alert = resp.IsSuccessStatusCode ? "Task added." : $"Add failed ({resp.StatusCode})";
            }
            else
            {
                resp = await Http.PutAsJsonAsync($"api/TicketManagment/tasks/{TaskId}", Model);
                Alert = resp.IsSuccessStatusCode ? "Task updated." : $"Update failed ({resp.StatusCode})";
            }

            Busy = false;

            if (resp.IsSuccessStatusCode)
                Nav.NavigateTo($"/managment/tasks/{ProjectId}");
        }

        protected void Cancel() => Nav.NavigateTo($"/managment/tasks/{ProjectId}");
    }
}
