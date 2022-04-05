namespace AdCore.Interface
{
    public interface ICacheService
    {
        void Set<T>(string key, T value);
        void Remove(string key);
        T Get<T>(string key);
    }
}
