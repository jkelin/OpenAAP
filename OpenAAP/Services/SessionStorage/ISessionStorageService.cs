using OpenAAP.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Services.Session
{
    public interface ISessionStorageService: IDisposable
    {
        Task StoreSession(SessionModel session);

        Task<SessionModel> LookupSessionBySessionId(Guid sessionId);

        Task<SessionModel[]> LookupSessionsByIdentityId(Guid identityId);

        Task DeleteSessionByIdentityId(Guid identityId);

        Task DeleteSessionBySessionId(Guid sessionId);
    }
}
