using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.DTOs;
using TaskManagmentClient.Services;


namespace TaskManagmentClient.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected NavigationManager Navigation { get; set; }
        [Inject] 
        private LoginState LoginState { get; set; }
        protected string Username { get; set; } = string.Empty;
        protected string Password { get; set; } = string.Empty;
        protected string Message { get; set; }
        protected string AlertClass { get; set; }
        protected bool IsBusy { get; set; }
     
        protected async Task TryLogin()
        {
            Message = string.Empty;
            AlertClass = string.Empty;
            IsBusy = true;

            var credentials = new LoginDto { Username = Username, Password = Password };
            HttpResponseMessage response;

            try
            {
                response = await Http.PostAsJsonAsync("api/TicketManagment/login", credentials);
            }
            catch (Exception ex)
            {
                Message = "Error contacting server: " + ex.Message;
                AlertClass = "alert-danger";
                IsBusy = false;
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                Message = $"Login successful! Welcome, {user.Username}.";
                AlertClass = "alert-success";
                LoginState.SetLogin(user.Username);
                Navigation.NavigateTo("/projects");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Message = "Invalid username or password.";
                AlertClass = "alert-warning";
            }
            else
            {
                Message = $"Server returned {response.StatusCode}";
                AlertClass = "alert-danger";
            }

            IsBusy = false;
        }

    }
}