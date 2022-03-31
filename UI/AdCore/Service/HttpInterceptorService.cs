using System.Net;
using System.Text.Json;
using AdCore.Exceptions;
using AdCore.Response;
using Microsoft.AspNetCore.Components;
using Toolbelt.Blazor;

namespace AdCore.Service
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly NavigationManager _navManager;
        private readonly ToastService _toastService;
        private readonly JsonSerializerOptions _options;

        public HttpInterceptorService(HttpClientInterceptor interceptor, 
            NavigationManager navManager, 
            ToastService toastService)
        {
            _interceptor = interceptor;
            _navManager = navManager;
            _toastService = toastService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public void RegisterEvent() => _interceptor.AfterSendAsync += InterceptResponse;

        private async Task InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
        {
            if (!e.Response.IsSuccessStatusCode)
            {
                var statusCode = e.Response.StatusCode;
                string message;
                switch (statusCode)
                {
                    case HttpStatusCode.NotFound:
                        // _navManager.NavigateTo("/404");
                        message = "The requested response was not found.";
                        break;
                    case HttpStatusCode.Unauthorized:
                        _navManager.NavigateTo("/login");
                        message = "User is not authorized";
                        break;
                    default:
                        // _navManager.NavigateTo("/500");
                        message = "Something went wrong, please contact Administrator";
                        break;
                }

                var responseContent = await e.Response.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _options);
                if (response != null)
                {
                    if(response.Errors != null && response.Errors.Any()) response.Errors.ForEach(Console.WriteLine);
                    if (response.InnerExceptions != null && response.InnerExceptions.Any()) response.InnerExceptions.ForEach(Console.WriteLine);
                    _toastService.ShowError(response.Message, 5000);
                }
                else
                {
                    _toastService.ShowError(message, 5000);
                }
                throw new HttpResponseException(message);
            }
        }

        public void DisposeEvent() => _interceptor.AfterSendAsync -= InterceptResponse;
    }
}
