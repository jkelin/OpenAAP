using Nito.AsyncEx;
using OpenAAP.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace OpenAAP.Services.Session
{
    public class InMemorySessionStorageService : ISessionStorageService
    {
        private Dictionary<Guid, ISet<Guid>> IdentityIdToSessions = new Dictionary<Guid, ISet<Guid>>();
        private Dictionary<Guid, Context.Session> SessionIdToSession = new Dictionary<Guid, Context.Session>();
        private AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();
        private Queue<(DateTime, Guid)> ExpiringSessions = new Queue<(DateTime, Guid)>();
        private Timer ExpirationClock;

        public InMemorySessionStorageService()
        {
            ExpirationClock = new Timer(100);
            ExpirationClock.Elapsed += async (a, b) => await Tick();
            ExpirationClock.Start();
        }

        /// <summary>
        /// InMemorySessionStorageService with timer disabled for testing purposes
        /// </summary>
        public InMemorySessionStorageService(bool disableTimer) : this()
        {
            ExpirationClock.Stop();
        }

        public void Dispose()
        {
            ExpirationClock.Dispose();
        }

        public async Task DeleteSessionByIdentityId(Guid identityId)
        {
            using (await Lock.WriterLockAsync())
            {
                if (IdentityIdToSessions.TryGetValue(identityId, out ISet<Guid> identitySet))
                {
                    foreach (var item in identitySet.ToArray())
                    {
                        DeleteSessionInner(item);
                    }
                }
            }
        }

        public async Task DeleteSessionBySessionId(Guid sessionId)
        {
            using (await Lock.WriterLockAsync())
            {
                DeleteSessionInner(sessionId);
            }
        }

        public async Task<Context.Session[]> LookupSessionsByIdentityId(Guid identityId)
        {
            using (await Lock.ReaderLockAsync())
            {
                if (IdentityIdToSessions.TryGetValue(identityId, out ISet<Guid> identitySet))
                {
                    return identitySet.Select(x => SessionIdToSession[x]).ToArray();
                }
                else
                {
                    return new Context.Session[0];
                }
            }
        }

        public async Task<Context.Session> LookupSessionBySessionId(Guid sessionId)
        {
            using (await Lock.ReaderLockAsync())
            {
                return SessionIdToSession.GetValueOrDefault(sessionId);
            }
        }

        public async Task StoreSession(Context.Session session)
        {
            using (await Lock.WriterLockAsync())
            {
                SessionIdToSession[session.Id] = session;

                ISet<Guid> sessions = IdentityIdToSessions.GetValueOrDefault(session.IdentityId) ?? (IdentityIdToSessions[session.IdentityId] = new HashSet<Guid>());
                sessions.Add(session.Id);
                ExpiringSessions.Enqueue((session.ExpiresAt, session.Id));
            }
        }

        /// <summary>
        /// Manually tick the timer. Useful for testing
        /// </summary>
        public async Task Tick()
        {
            using (var l = await Lock.WriterLockAsync())
            {
                // Delete each expired session
                while (ExpiringSessions.TryPeek(out (DateTime, Guid) y) && y.Item1 < DateTime.UtcNow)
                {
                    DeleteSessionInner(ExpiringSessions.Dequeue().Item2);
                }
            }
        }

        // Write lock needs to be engaged before calling this
        private void DeleteSessionInner(Guid sessionId)
        {
            if(SessionIdToSession.TryGetValue(sessionId, out Context.Session session))
            {
                SessionIdToSession.Remove(sessionId);

                if(IdentityIdToSessions.TryGetValue(session.IdentityId, out ISet<Guid> identitySet))
                {
                    identitySet.Remove(sessionId);
                    if(identitySet.Count == 0)
                    {
                        IdentityIdToSessions.Remove(session.IdentityId);
                    }
                }
            }
        }
    }
}
