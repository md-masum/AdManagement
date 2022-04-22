using AdCore.Models.Auth;
using Microsoft.AspNetCore.Components;

namespace AdUi.Pages.Admin.user
{
    public partial class UserUpdate : IDisposable
    {
        [Parameter]
        public string UserId { get; set; }
        public UserModel UserModel { get; set; }
        protected override void OnInitialized()
        {
            _store.OnChange += StateHasChanged;
            _httpInterceptor.RegisterEvent();

            
        }

        protected override async Task OnParametersSetAsync()
        {
            var user = await _httpAuthorizeClient.GetAsync<UserDto>($"/api/auth/{UserId}");
            if (user is not null)
            {
                UserModel = _mapper.Map<UserModel>(user);
            }
        }

        public async Task UpdateUser()
        {
            var user = _mapper.Map<UserDto>(UserModel);
            await _httpAuthorizeClient.PutAsync<object, UserDto>($"/api/auth/{UserId}", user);
            _toastService.ShowSuccess("User Updated Successfully", 5000);
        }

        public void Dispose()
        {
            _store.OnChange -= StateHasChanged;
            _httpInterceptor.DisposeEvent();
        }
    }
}
