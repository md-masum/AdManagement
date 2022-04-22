using AdCore.Models.UserProfile;

namespace AdUi.Pages.Seller.Profiles
{
    public partial class Profile
    {
        public ProfileModel CurrentUser { get; set; } = new();
        private bool _isEditMode;

        private string _password;
        protected override async Task OnInitializedAsync()
        {
            await GetUser();
        }

        private async Task UpdateUser()
        {
            var updatedUser =
                await _httpAuthorizeClient.PostAsync<ProfileModel, ProfileUpdateModel>("/api/user/updateCurrentUser",
                    _mapper.Map<ProfileUpdateModel>(CurrentUser), true);
            if (updatedUser is not null)
            {
                CurrentUser = updatedUser;
                _isEditMode = false;
                _toastService.ShowSuccess("User Updated Successfully", 5000);
                StateHasChanged();
            }
        }

        private async Task ChangeUserPassword()
        {
            if (_password is null)
            {
                _toastService.ShowWarn("Password can't be null",  5000);
            }
            else
            {
                var updatedUser =
                    await _httpAuthorizeClient.PostAsync<bool, string>("/api/user/changeUserPassword",
                        _password);
                if (updatedUser)
                {
                    _toastService.ShowSuccess("Password Updated Successfully", 5000);
                }
            }
            
        }

        private async Task GetUser()
        {
            var user = await _httpAuthorizeClient.GetAsync<ProfileModel>("/api/user/getCurrentUser");
            if (user is null)
            {
                _navigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
            }
            else
            {
                CurrentUser = user;
            }
        }

        private async Task OnCancelCLick()
        {
            _isEditMode = false;
            await GetUser();
        }
        private void OnEditCLick()
        {
            _isEditMode = true;
        }
    }
}
