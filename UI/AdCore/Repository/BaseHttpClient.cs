using System.Net.Http.Json;
using AdCore.Response;
using AdCore.Service;
using AdCore.Store;

namespace AdCore.Repository
{
    public class BaseHttpClient
    {
        private readonly HttpClient _client;
        private readonly ToastService _toastService;
        private readonly StoreContainer _store;

        public BaseHttpClient(HttpClient client, ToastService toastService, StoreContainer store)
        {
            _client = client;
            _toastService = toastService;
            _store = store;
        }

        public virtual async Task<TResponse> GetAsync<TResponse>(string url)
        {
            try
            {
                _store.IsLoading = true;
                var response = await _client.GetFromJsonAsync<ApiResponse<TResponse>>(url);
                _store.IsLoading = false;
                return response!.Data;
            }
            catch (Exception e)
            {
                _store.IsLoading = false;
                Console.WriteLine(e);
                _toastService.ShowError(e.Message, 5000);
                throw;
            }
            
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string url, TRequest requestBody)
        {
            try
            {
                _store.IsLoading = true;
                var response = await _client.PostAsJsonAsync(url, requestBody);
                _store.IsLoading = false;
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                return content!.Data;
            }
            catch (Exception e)
            {
                _store.IsLoading = false;
                Console.WriteLine(e);
                _toastService.ShowError(e.Message, 5000);
                throw;
            }

        }

        public async Task<TResponse> PutAsync<TResponse, TRequest>(string url, TRequest requestBody)
        {
            try
            {
                _store.IsLoading = true;
                var response = await _client.PutAsJsonAsync(url, requestBody);
                _store.IsLoading = false;
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                return content!.Data;
            }
            catch (Exception e)
            {
                _store.IsLoading = false;
                Console.WriteLine(e);
                _toastService.ShowError(e.Message, 5000);
                throw;
            }

        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                _store.IsLoading = true;
                var response = await _client.DeleteAsync(url);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                _store.IsLoading = false;
                return content!.Data;
            }
            catch (Exception e)
            {
                _store.IsLoading = false;
                Console.WriteLine(e);
                _toastService.ShowError(e.Message, 5000);
                throw;
            }

        }
    }
}
