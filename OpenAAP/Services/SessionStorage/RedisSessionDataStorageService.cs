using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAAP.Context;
using OpenAAP.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Services.SessionDataStorage
{
    public class RedisSessionDataStorageService : ISessionDataStorage
    {
        private readonly SessionOptions sessionOptions;
        private readonly IDatabase redis;

        public RedisSessionDataStorageService(IOptions<SessionOptions> sessionOptions, IDatabase redis)
        {
            this.sessionOptions = sessionOptions.Value;
            this.redis = redis;
        }

        public async Task DeleteSession(Guid sessionId)
        {
            await redis.KeyDeleteAsync(SessionKey(sessionId));
        }

        public async Task<ISession> GetSession(Guid sessionId)
        {
            var data = await redis.StringGetAsync(SessionKey(sessionId));

            if (data.HasValue)
            {
                return JsonConvert.DeserializeObject<Session>(data.ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task StoreSession(Guid sessionId, ISession session)
        {
            var dataString = JsonConvert.SerializeObject(session);

            await redis.StringSetAsync(SessionKey(sessionId), dataString, session.ExpiresAt - DateTime.UtcNow);
        }

        private RedisKey SessionKey(Guid sessionId)
        {
            return "session:" + sessionId.ToString();
        }
    }
}
