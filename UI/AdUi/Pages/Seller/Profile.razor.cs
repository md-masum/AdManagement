using AdCore.Constant;
using AdCore.Models.Auth;

namespace AdUi.Pages.Seller
{
    public partial class Profile
    {
        public UserDto CurrentUser { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var userId = _js.Invoke<string>(JsInteropConstant.GetSessionStorage, JsInteropConstant.CurrentUserId);
            var user = await _httpAuthorizeClient.GetAsync<UserDto>($"/api/auth/{userId}");
            if (user is null)
            {
                _navigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
            }
            else
            {
                CurrentUser = user;
            }
        }
    }
}
