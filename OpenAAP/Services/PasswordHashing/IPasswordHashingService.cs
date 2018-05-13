using OpenAAP.Options;
using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    public interface IPasswordHashingService
    {
        Task<byte[]> Hash(byte[] password, byte[] salt, TargetHashConfigration config);
    }
}
