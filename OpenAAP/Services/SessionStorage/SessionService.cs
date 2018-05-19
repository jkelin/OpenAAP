using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAAP.Context;
using OpenAAP.Options;
using OpenAAP.Services.SessionDataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Services.SessionStorage
{
    public class SessionService
    {
        private readonly OpenAAPContext context;
        private readonly IOptions<SessionOptions> sessionOptions;
        private readonly ISessionDataStorage storage;

        public SessionService(
            OpenAAPContext context,
            IOptions<SessionOptions> sessionOptions,
            ISessionDataStorage storage
        )
        {
            this.context = context;
            this.sessionOptions = sessionOptions;
            this.storage = storage;
        }

        public async Task<ISession> CreateSession(Guid identityId, object data = null)
        {
            if (data == null)
            {
                data = new
                {
                };
            }

            var session = new Session
            {
                Id = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddMilliseconds(sessionOptions.Value.ExpirationMs),
                IdentityId = identityId,
                Data = data
            };

            await context.Sessions.AddAsync(session);

            await Task.WhenAll(new[]
            {
                storage.StoreSession(session.Id, session),
                context.SaveChangesAsync()
            });

            return session;
        }

        public async Task<ISession> UpdateSession(Guid sessionId, object data)
        {
            var session = await LookupBySessionId(sessionId);
            if(session == null)
            {
                return null;
            }

            session.Data = data;

            await storage.StoreSession(sessionId, session);
            return session;
        }

        public async Task DeleteSessionsForIdentity(Guid identityId)
        {
            var identity = await context.Identities.Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == identityId);
            if (identity != null)
            {
                var tasks = new List<Task>(identity.Sessions.Count + 1);

                foreach (var session in identity.Sessions)
                {
                    context.Sessions.Remove(session);
                    tasks.Add(storage.DeleteSession(session.Id));
                }

                tasks.Add(context.SaveChangesAsync());

                await Task.WhenAll(tasks);
            }
        }

        public Task<ISession> LookupBySessionId(Guid sessionId)
        {
            return storage.GetSession(sessionId);
        }

        public async Task DeleteBySessionId(Guid sessionId)
        {
            var sessionInCtx = await context.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
            if(sessionInCtx != null)
            {
                context.Sessions.Remove(sessionInCtx);
            }

            await Task.WhenAll(new []{
                storage.DeleteSession(sessionId),
                context.SaveChangesAsync()
            });
        }
    }
}
