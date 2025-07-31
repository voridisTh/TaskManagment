using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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
    public class EditProjectBase : ComponentBase
    {
        [Parameter] public int Id { get; set; }

        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected ProjectDto Model { get; set; } = new();
        protected List<StatusDto> Statuses = new();

        protected bool Busy { get; set; }
        protected bool IsLoaded { get; set; }
        protected string? Alert { get; set; }
        protected bool IsNew => Id == 0;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Statuses = await Http.GetFromJsonAsync<List<StatusDto>>("api/TicketManagment/statuses")
                           ?? new List<StatusDto>();

                if (!IsNew)
                {
                    var project = await Http.GetFromJsonAsync<ProjectDto>($"api/TicketManagment/projects/{Id}");
                    if (project != null)
                    {
                        Model = project;
                    }
                    else
                    {
                        Alert = "Project not found.";
                    }
                }

                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Alert = $"Error loading data: {ex.Message}";
                IsLoaded = true;
            }
        }

        protected async Task SaveAsync()
        {
            Busy = true;
            HttpResponseMessage resp;

            if (IsNew)
            {
                resp = await Http.PostAsJsonAsync("api/TicketManagment/projects", Model);
                Alert = resp.IsSuccessStatusCode ? "Project added." : $"Add failed ({resp.StatusCode})";
            }
            else
            {
                var updateDto = new UpdateProjectDto
                {
                    Id = Model.Id,
                    Name = Model.Name,
                    Description = Model.Description,
                    StatusId = Model.StatusId
                };

                resp = await Http.PutAsJsonAsync($"api/TicketManagment/projects/{Model.Id}", updateDto);
                Alert = resp.IsSuccessStatusCode ? "Project updated." : $"Update failed ({resp.StatusCode})";
            }

            Busy = false;

            if (resp.IsSuccessStatusCode)
                Nav.NavigateTo("/managment");
        }

        protected void Cancel() => Nav.NavigateTo("/managment");
    }
}
