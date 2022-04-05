using AdCore.Constant;
using AdCore.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace AdCore.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _semaphore;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
            _semaphore = AppConstant.Semaphore;
        }
        public void Set<T>(string key, T value)
        {
            _semaphore.Wait();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal)
                .SetSize(1024);

            _cache.Set(key, value, cacheEntryOptions);

            _semaphore.Release();
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }
    }
}
