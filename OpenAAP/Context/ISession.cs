using System;

namespace OpenAAP.Context
{
    public interface ISession
    {
        object Data { get; set; }
        DateTime ExpiresAt { get; set; }
        Guid Id { get; set; }
        Guid IdentityId { get; set; }
    }
}
