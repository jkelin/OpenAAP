using System;
using System.Threading.Tasks;
using OpenAAP.Context;

namespace OpenAAP.Services.SessionDataStorage
{
    public interface ISessionDataStorage
    {
        Task StoreSession(Guid sessionId, ISession session);

        Task<ISession> GetSession(Guid sessionId);

        Task DeleteSession(Guid sessionId);
    }
}
