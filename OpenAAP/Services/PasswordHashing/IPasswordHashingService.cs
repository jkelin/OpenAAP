using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    public interface IPasswordHashingService
    {
        Task<byte[]> Hash(byte[] password, byte[] salt, PasswordAuthenticationHashAlgorithm algorithm);
    }
}
