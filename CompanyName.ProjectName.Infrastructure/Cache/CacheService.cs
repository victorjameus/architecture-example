using CompanyName.ProjectName.Application.Common.Interfaces;

namespace CompanyName.ProjectName.Infrastructure.Cache;

internal sealed class CacheService(IMemoryCache cache) : ICacheService
{
    public T? Get<T>(string key)
    {
        cache.TryGetValue(key, out T? value);

        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        cache.Set
        (
            key,
            value,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
        );
    }

    public void Remove(string key)
    {
        cache.Remove(key);
    }
}