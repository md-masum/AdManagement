using AdCore.Models.Auth;

namespace AdUi.Pages.Admin
{
    public partial class UserList : IDisposable
    {
        public List<UserDto> ApplicationUsers { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _store.OnChange += StateHasChanged;
            _httpInterceptor.RegisterEvent();

            var userList = await _httpAuthorizeClient.GetAsync<List<UserDto>>("/api/auth");
            ApplicationUsers = userList is not null && userList.Any() ? userList : new List<UserDto>();
        }

        public void Dispose()
        {
            _store.OnChange -= StateHasChanged;
            _httpInterceptor.DisposeEvent();
        }
    }
}
