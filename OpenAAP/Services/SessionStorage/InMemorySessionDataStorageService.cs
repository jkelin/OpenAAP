using Microsoft.Extensions.Caching.Memory;
using Nito.AsyncEx;
using OpenAAP.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace OpenAAP.Services.SessionDataStorage
{
    public class InMemorySessionDataStorageService : ISessionDataStorage, IDisposable
    {
        private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private readonly MemoryCache expirationCache = new MemoryCache(new MemoryCacheOptions());

        public void Dispose()
        {
            cache.Dispose();
            expirationCache.Dispose();
        }

        public Task DeleteSession(Guid sessionId)
        {
            cache.Remove(sessionId);
            expirationCache.Remove(sessionId);
            return Task.CompletedTask;
        }

        public Task<ISession> GetSession(Guid sessionId)
        {
            if (cache.TryGetValue(sessionId, out ISession x))
            {
                return Task.FromResult(x);
            }
            else
            {
                return Task.FromResult<ISession>(null);
            }
        }

        public Task StoreSession(Guid sessionId, ISession session)
        {
            cache.Set(sessionId, session, session.ExpiresAt);
            expirationCache.Set(sessionId, session.ExpiresAt, session.ExpiresAt);

            return Task.CompletedTask;
        }
    }
}
