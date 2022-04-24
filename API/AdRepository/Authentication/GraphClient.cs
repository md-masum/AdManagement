﻿using System.Text.Json;
using AdCore.Constant;
using AdCore.Dto.Users;
using AdCore.Enums;
using AdCore.Exceptions;
using AdCore.Helpers;
using AdCore.Interface;
using AdCore.Settings;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace AdRepository.Authentication
{
    //Configure B2C Tenant https://docs.microsoft.com/en-us/azure/active-directory-b2c/microsoft-graph-get-started?tabs=app-reg-ga
    //Configure App to call graph https://docs.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=CS#client-credentials-provider
    //Sample Code https://github.com/Azure-Samples/ms-identity-dotnetcore-b2c-account-management/blob/master/src/Services/UserService.cs
    public class GraphClient
    {
        private readonly B2CCredentials _credentials;
        private readonly ILogger<GraphClient> _logger;
        private readonly ICacheService _cacheService;
        public virtual GraphServiceClient GraphServiceClient { get; }
        public GraphClient(B2CCredentials credentials, ILogger<GraphClient> logger, ICacheService cacheService)
        {
            _credentials = credentials;
            _logger = logger;
            _cacheService = cacheService;
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            
            var clientSecretCredential = new ClientSecretCredential(
                credentials.TenantId, credentials.ClientId, credentials.ClientSecret, options);

            GraphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
        }

        public async Task<IList<UserDto>> GetAllUser()
        {
            string roleAttributeName = GetAttributeFullName("Role");
            var users = await GraphServiceClient.Users
                .Request()
                .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                .GetAsync();

            return users.Select(GetUserEntity).ToList();
        }

        public async Task<UserDto> GetUserById(string userId)
        {
            try
            {
                var user = GetUserEntity(await GetUser(userId));
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<string> GetUserRoleById(string userId)
        {
            var user = await GetUser(userId);
            string roleAttributeName = GetAttributeFullName("Role");
            return user.AdditionalData.TryGetValue(roleAttributeName, out var role) ? role.ToString() : string.Empty;
        }

        public async Task UpdateUser(string userId, UserUpdateModel userToUpdate)
        {
            string roleAttributeName = GetAttributeFullName("Role");
            var extensionInstance = RoleExtensionInstance(userToUpdate.Role);

            var user = new User
            {
                GivenName = userToUpdate.FirstName,
                Surname = userToUpdate.LastName,
                DisplayName = userToUpdate.DisplayName,
                AdditionalData = extensionInstance
            };
            await GraphServiceClient.Users[userId]
                .Request()
                .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                .UpdateAsync(user);
            _cacheService.Remove(nameof(UserDto));
            _cacheService.Remove(CacheKey.UserList);
        }

        public async Task<bool> AddUserRole(string userId, Roles roles)
        {
            try
            {
                var user = await GetUser(userId);

                if (user.IsInRole(_credentials.B2CExtensionAppClientId, "Admin")
                    || user.IsInRole(_credentials.B2CExtensionAppClientId, "Seller")
                    || user.IsInRole(_credentials.B2CExtensionAppClientId, "User")) return false;

                string roleAttributeName = GetAttributeFullName("Role");
                var extensionInstance = RoleExtensionInstance(roles);

                var userRoleAdd = new User
                {
                    AdditionalData = extensionInstance
                };

                await GraphServiceClient.Users[userId]
                    .Request()
                    .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                    .UpdateAsync(userRoleAdd);

                _cacheService.Remove(nameof(UserDto));
                _cacheService.Remove(CacheKey.UserList);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
        }

        public async Task<IList<UserDto>> ListUsers()
        {
            _logger.LogInformation("Getting list of users...");
            string roleAttributeName = GetAttributeFullName("Role");

            List<UserDto> userList = new List<UserDto>();
            var cacheData = _cacheService.Get<List<UserDto>>(CacheKey.UserList);
            if (cacheData is not null) return cacheData;
            try
            {
                var users = await GraphServiceClient.Users
                    .Request()
                    .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                    .GetAsync();
                
                var pageIterator = PageIterator<User>
                    .CreatePageIterator(
                        GraphServiceClient,
                        users,
                        (user) =>
                        {
                            _logger.LogInformation(user.DisplayName);
                            userList.Add(GetUserEntity(user));
                            return true;
                        },
                        (req) =>
                        {
                            _logger.LogInformation($"Reading next page of users...");
                            return req;
                        }
                    );

                await pageIterator.IterateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new CustomException("Something went wrong to retrieve user list", ex);
            }
            _cacheService.Set(CacheKey.UserList, userList);
            return userList;
        }

        public async Task DeleteUserById(string userId)
        {
            _logger.LogInformation($"Looking for user with object ID '{userId}'...");
            try
            {
                // Delete user by object ID
                await GraphServiceClient.Users[userId]
                    .Request()
                    .DeleteAsync();
                _cacheService.Remove(nameof(UserDto));
                _cacheService.Remove(CacheKey.UserList);

                _logger.LogInformation($"User with object ID '{userId}' successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new CustomException("Something went wrong to delete user by ID", ex);
            }
        }

        public async Task SetPasswordByUserId(string userId, string password)
        {
            _logger.LogInformation($"Looking for user with object ID '{userId}'...");

            var user = new User
            {
                PasswordPolicies = "DisablePasswordExpiration,DisableStrongPassword",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = password,
                }
            };

            // Update user by object ID
            await GraphServiceClient.Users[userId]
                .Request()
                .UpdateAsync(user);
            Console.WriteLine($"User with object ID '{userId}' successfully updated.");
        }

        public async Task<UserDto> CreateUserWithCustomAttribute(UserModel createUserObj)
        {
            if (string.IsNullOrWhiteSpace(_credentials.B2CExtensionAppClientId))
            {
                throw new ArgumentException("B2C Extension App ClientId (ApplicationId) is missing in the appsettings.json. Get it from the App Registrations blade in the Azure portal. The app registration has the name 'b2c-extensions-app. Do not modify. Used by AADB2C for storing user data.'.", nameof(GraphClient));
            }

            string roleAttributeName = GetAttributeFullName("Role");
            var extensionInstance = RoleExtensionInstance(createUserObj.Role);

            try
            {
                // Create user
                var result = await GraphServiceClient.Users
                .Request()
                .AddAsync(new User
                {
                    GivenName = createUserObj.FirstName,
                    Surname = createUserObj.LastName,
                    DisplayName = createUserObj.DisplayName,
                    Identities = new List<ObjectIdentity>
                    {
                        new ObjectIdentity()
                        {
                            SignInType = "emailAddress",
                            Issuer = _credentials.Domain,
                            IssuerAssignedId = createUserObj.Email
                        }
                    },
                    PasswordProfile = new PasswordProfile()
                    {
                        Password = createUserObj.Password
                    },
                    PasswordPolicies = "DisablePasswordExpiration",
                    AdditionalData = extensionInstance
                });

                string userId = result.Id;

                _logger.LogInformation($"Created the new user. Now get the created user with object ID '{userId}'...");

                // Get created user by object ID
                result = await GraphServiceClient.Users[userId]
                    .Request()
                    .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                    .GetAsync();

                if (result != null)
                {
                    _logger.LogInformation($"DisplayName: {result.DisplayName}");
                    _logger.LogInformation($"Role: {result.AdditionalData[roleAttributeName]}");
                    _logger.LogInformation(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
                }
                _cacheService.Remove(nameof(UserDto));
                _cacheService.Remove(CacheKey.UserList);
                return GetUserEntity(result);
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogError($"Have you created the custom attributes in your tenant?");
                    _logger.LogError(ex.Message);
                    throw new CustomException("Something went wrong to create user, Have you created the custom attributes in your tenant?", ex);
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new CustomException("Something went wrong to create user", ex);
            }
        }

        #region Helper Method
        private UserDto GetUserEntity(User userObject)
        {
            return new UserDto
            {
                Id = userObject.Id,
                FirstName = userObject.GivenName,
                LastName = userObject.Surname,
                DisplayName = userObject.DisplayName,
                Email = userObject.Identities.FirstOrDefault(c => c.SignInType == "emailAddress")?.IssuerAssignedId,
                Role = (Roles)Enum.Parse(typeof(Roles), GetUserRole(userObject))
            };
        }
        private string GetUserRole(User user)
        {
            string roleAttributeName = GetAttributeFullName("Role");
            return user.AdditionalData.TryGetValue(roleAttributeName, out var role) ? role.ToString() : string.Empty;
        }
        private async Task<User> GetUser(string userId)
        {
            string roleAttributeName = GetAttributeFullName("Role");
            var user = await GraphServiceClient.Users[userId]
                .Request()
                .Select($"id,givenName,surName,displayName,identities,{roleAttributeName}")
                .GetAsync();
            if (user == null) throw new NotFoundException("No User found by provided ID");
            return user;
        }
        private string GetAttributeFullName(string attributeName)
        {
            B2CCustomAttributeHelper helper = new B2CCustomAttributeHelper(_credentials.B2CExtensionAppClientId);
            return helper.GetCompleteAttributeName(attributeName);
        }
        private IDictionary<string, object> RoleExtensionInstance(Roles roles)
        {
            string roleAttributeName = GetAttributeFullName("Role");

            IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
            extensionInstance.Add(roleAttributeName, roles.ToString());
            return extensionInstance;
        }
        #endregion

    }
}
