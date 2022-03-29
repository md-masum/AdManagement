using System.Net.Http.Json;
using AdUi.Service;
using AdUi.Store;

namespace AdUi.Repository
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

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            try
            {
                _store.IsLoading = true;
                var response = await _client.GetFromJsonAsync<TResponse>(url);
                _store.IsLoading = false;
                return response;
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
                var content = await response.Content.ReadFromJsonAsync<TResponse>();
                _store.IsLoading = false;
                return content;
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
                var content = await response.Content.ReadFromJsonAsync<TResponse>();
                _store.IsLoading = false;
                return content;
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
                _store.IsLoading = false;
                return response.IsSuccessStatusCode;
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
