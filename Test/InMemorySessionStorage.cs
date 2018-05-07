using OpenAAP.Context;
using OpenAAP.Services.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace Test
{
    public class InMemorySessionStorage
    {
        [Fact]
        public async Task LookupSessionBySessionId()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);
                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);

                Assert.Equal(sessionIn.SessionId, sessionOut.SessionId);
                Assert.Equal(sessionIn.IdentityId, sessionOut.IdentityId);
                Assert.Equal(sessionIn.ExpiresAt, sessionOut.ExpiresAt);
            }
        }

        [Fact]
        public async Task LookupSessionBySessionIdUnknown()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionOut = await store.LookupSessionBySessionId(Guid.NewGuid());

                Assert.Equal(null, sessionOut);
            }
        }

        [Fact]
        public async Task LookupSessionBySessionIdMultipleSameIdentity()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);

                for (int i = 0; i < 10; i++)
                {
                    var sessionInAdditional = new SessionModel
                    {
                        IdentityId = sessionIn.IdentityId,
                        SessionId = Guid.NewGuid(),
                        ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                    };

                    await store.StoreSession(sessionInAdditional);
                }

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);

                Assert.Equal(sessionIn.SessionId, sessionOut.SessionId);
                Assert.Equal(sessionIn.IdentityId, sessionOut.IdentityId);
                Assert.Equal(sessionIn.ExpiresAt, sessionOut.ExpiresAt);
            }
        }

        [Fact]
        public async Task LookupSessionBySessionIdMultipleMultipleIdentities()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);

                for (int i = 0; i < 10; i++)
                {
                    var sessionInAdditional = new SessionModel
                    {
                        IdentityId = Guid.NewGuid(),
                        SessionId = Guid.NewGuid(),
                        ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                    };

                    await store.StoreSession(sessionInAdditional);
                }

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);

                Assert.Equal(sessionIn.SessionId, sessionOut.SessionId);
                Assert.Equal(sessionIn.IdentityId, sessionOut.IdentityId);
                Assert.Equal(sessionIn.ExpiresAt, sessionOut.ExpiresAt);
            }
        }

        [Fact]
        public async Task LookupSessionsByIdentityIdSingle()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);
                var sessionsOut = await store.LookupSessionsByIdentityId(sessionIn.IdentityId);

                Assert.Equal(1, sessionsOut.Length);
                Assert.Equal(sessionIn.SessionId, sessionsOut[0].SessionId);
                Assert.Equal(sessionIn.IdentityId, sessionsOut[0].IdentityId);
                Assert.Equal(sessionIn.ExpiresAt, sessionsOut[0].ExpiresAt);
            }
        }

        [Fact]
        public async Task LookupSessionsByIdentityIdUnknown()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionsOut = await store.LookupSessionsByIdentityId(Guid.NewGuid());

                Assert.Equal(0, sessionsOut.Length);
            }
        }

        [Fact]
        public async Task LookupSessionsByIdentityIdMultiple()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionsIn = new List<SessionModel>();
                var identityId = Guid.NewGuid();

                for (int i = 0; i < 10; i++)
                {
                    var sessionIn = new SessionModel
                    {
                        IdentityId = identityId,
                        SessionId = Guid.NewGuid(),
                        ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                    };

                    await store.StoreSession(sessionIn);
                    sessionsIn.Add(sessionIn);
                }

                var sessionsOut = await store.LookupSessionsByIdentityId(identityId);

                Assert.Equal(10, sessionsOut.Length);

                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(sessionsIn[i].SessionId, sessionsOut[i].SessionId);
                    Assert.Equal(sessionsIn[i].IdentityId, sessionsOut[i].IdentityId);
                    Assert.Equal(sessionsIn[i].ExpiresAt, sessionsOut[i].ExpiresAt);
                }
            }
        }

        [Fact]
        public async Task DeleteSessionBySessionId()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);
                await store.DeleteSessionBySessionId(sessionIn.SessionId);
                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);

                Assert.Null(sessionOut);
            }
        }

        [Fact]
        public async Task DeleteSessionBySessionIdUnknown()
        {
            using (var store = new InMemorySessionStorageService())
            {
                await store.DeleteSessionBySessionId(Guid.NewGuid());
            }
        }

        [Fact]
        public async Task DeleteSessionBySessionIdTwo()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn);

                var sessionIn2 = new SessionModel
                {
                    IdentityId = sessionIn.IdentityId,
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn2);

                await store.DeleteSessionBySessionId(sessionIn.SessionId);

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.Null(sessionOut);

                var sessionOut2 = await store.LookupSessionBySessionId(sessionIn2.SessionId);
                Assert.Equal(sessionIn2.SessionId, sessionOut2.SessionId);
                Assert.Equal(sessionIn2.IdentityId, sessionOut2.IdentityId);
                Assert.Equal(sessionIn2.ExpiresAt, sessionOut2.ExpiresAt);
            }
        }

        [Fact]
        public async Task DeleteSessionBySessionIdTwoDifferentIdentities()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn);

                var sessionIn2 = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn2);

                await store.DeleteSessionBySessionId(sessionIn.SessionId);

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.Null(sessionOut);

                var sessionOut2 = await store.LookupSessionBySessionId(sessionIn2.SessionId);
                Assert.Equal(sessionIn2.SessionId, sessionOut2.SessionId);
                Assert.Equal(sessionIn2.IdentityId, sessionOut2.IdentityId);
                Assert.Equal(sessionIn2.ExpiresAt, sessionOut2.ExpiresAt);
            }
        }

        [Fact]
        public async Task DeleteSessionByIdentityId()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };

                await store.StoreSession(sessionIn);
                await store.DeleteSessionByIdentityId(sessionIn.IdentityId);
                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);

                Assert.Null(sessionOut);
            }
        }

        [Fact]
        public async Task DeleteSessionByIdentityIdUnknown()
        {
            using (var store = new InMemorySessionStorageService())
            {
                await store.DeleteSessionByIdentityId(Guid.NewGuid());
            }
        }

        [Fact]
        public async Task DeleteSessionByIdentityIdTwo()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn);

                var sessionIn2 = new SessionModel
                {
                    IdentityId = sessionIn.IdentityId,
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn2);

                await store.DeleteSessionByIdentityId(sessionIn.IdentityId);

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.Null(sessionOut);

                var sessionOut2 = await store.LookupSessionBySessionId(sessionIn2.SessionId);
                Assert.Null(sessionOut2);
            }
        }

        [Fact]
        public async Task DeleteSessionByIdentityIdTwoDifferentIdentities()
        {
            using (var store = new InMemorySessionStorageService())
            {
                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn);

                var sessionIn2 = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddSeconds(10),
                };
                await store.StoreSession(sessionIn2);

                await store.DeleteSessionByIdentityId(sessionIn.IdentityId);

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.Null(sessionOut);

                var sessionOut2 = await store.LookupSessionBySessionId(sessionIn2.SessionId);
                Assert.Equal(sessionIn2.SessionId, sessionOut2.SessionId);
                Assert.Equal(sessionIn2.IdentityId, sessionOut2.IdentityId);
                Assert.Equal(sessionIn2.ExpiresAt, sessionOut2.ExpiresAt);
            }
        }

        [Fact]
        public async Task Expiration()
        {
            using (var store = new InMemorySessionStorageService(true))
            {
                // Warmup start
                for (int i = 0; i < 10; i++)
                {
                    await store.Tick();
                    var sessionInWarmup = new SessionModel
                    {
                        IdentityId = Guid.NewGuid(),
                        SessionId = Guid.NewGuid(),
                        ExpiresAt = DateTime.UtcNow.AddMilliseconds(500),
                    };

                    await store.StoreSession(sessionInWarmup);
                    await Task.Delay(10);
                    await store.Tick();

                    var sessionOutWarmup = await store.LookupSessionBySessionId(sessionInWarmup.SessionId);
                }
                // Warmup end


                var sessionIn = new SessionModel
                {
                    IdentityId = Guid.NewGuid(),
                    SessionId = Guid.NewGuid(),
                    ExpiresAt = DateTime.UtcNow.AddMilliseconds(500),
                };

                await store.StoreSession(sessionIn);

                await store.Tick();
                await Task.Delay(100);
                await store.Tick();

                var sessionOut = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.NotNull(sessionOut);
                Assert.Equal(sessionIn.SessionId, sessionOut.SessionId);
                Assert.Equal(sessionIn.IdentityId, sessionOut.IdentityId);
                Assert.Equal(sessionIn.ExpiresAt, sessionOut.ExpiresAt);

                await store.Tick();
                await Task.Delay(600);
                await store.Tick();
  
                var sessionOut2 = await store.LookupSessionBySessionId(sessionIn.SessionId);
                Assert.Null(sessionOut2);
            }
        }
    }
}
