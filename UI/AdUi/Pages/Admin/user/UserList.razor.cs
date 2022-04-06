using AdCore.Models.Auth;
using AdUi.Components;

namespace AdUi.Pages.Admin.user
{
    public partial class UserList : IDisposable
    {
        public List<UserDto> ApplicationUsers { get; set; }
        protected Confirm DeleteConfirmation { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            _store.OnChange += StateHasChanged;
            _httpInterceptor.RegisterEvent();

            var userList = await _httpAuthorizeClient.GetAsync<List<UserDto>>("/api/auth");
            ApplicationUsers = userList is not null && userList.Any() ? userList : new List<UserDto>();
        }

        private void OnUserEdit(string userId)
        {
            _navigationManager.NavigateTo($"/admin/user/{userId}");
        }

        private async Task OnUserDelete(string userId)
        {
            await _httpAuthorizeClient.DeleteAsync($"/api/auth/{userId}");
            var deletedUser = ApplicationUsers.FirstOrDefault(c => c.Id == userId);
            if (deletedUser is not null)
            {
                ApplicationUsers.Remove(deletedUser);
            }
        }

        private void DeleteCLick(string userId)
        {
            DeleteConfirmation.Show(userId);
        }

        public void Dispose()
        {
            _store.OnChange -= StateHasChanged;
            _httpInterceptor.DisposeEvent();
        }
    }
}
