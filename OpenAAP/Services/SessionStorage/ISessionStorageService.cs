using OpenAAP.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Services.Session
{
    public interface ISessionStorageService: IDisposable
    {
        Task StoreSession(Context.Session session);

        Task<Context.Session> LookupSessionBySessionId(Guid sessionId);

        Task<Context.Session[]> LookupSessionsByIdentityId(Guid identityId);

        Task DeleteSessionByIdentityId(Guid identityId);

        Task DeleteSessionBySessionId(Guid sessionId);
    }
}
