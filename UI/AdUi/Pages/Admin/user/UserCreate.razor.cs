using AdCore.Models.Auth;
using Newtonsoft.Json;

namespace AdUi.Pages.Admin.user
{
    public partial class UserCreate
    {
        public UserModel UserModel = new UserModel();

        public async Task CreateUser()
        {
            var newUser = await _httpAuthorizeClient.PostAsync<UserDto, UserModel>("/api/auth", UserModel);
            _toastService.ShowSuccess("User Created Successfully", 5000);
            Console.WriteLine(JsonConvert.SerializeObject(newUser));
            _navigationManager.NavigateTo("/admin/user");
        }
    }
}
