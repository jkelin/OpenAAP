using OpenAAP.Services.PasswordHashing;
using ProtoBuf;
using System.IO;

namespace OpenAAP.Options
{
    public enum HashingAlgorithm
    {
        SHA1,
        SHA256,
        SHA512,
        PBKDF2,
    }

    [ProtoContract]
    public class TargetHashConfigration
    {
        [ProtoMember(11)]
        public HashingAlgorithm? Algorithm { get; set; } = HashingAlgorithm.PBKDF2;

        [ProtoMember(12)]
        public HashingAlgorithm? Algorithm2 { get; set; } = HashingAlgorithm.SHA256;

        [ProtoMember(13)]
        public int? SaltLength { get; set; } = 32;

        [ProtoMember(14)]
        public int? IterationCount { get; set; } = 1024;

        [ProtoMember(15)]
        public int? PasswordHashBytes { get; set; } = 32;

        public bool ConfigEquals(TargetHashConfigration other) => Algorithm == other.Algorithm
            && Algorithm2 == other.Algorithm2
            && SaltLength == other.SaltLength
            && IterationCount == other.IterationCount
            && PasswordHashBytes == other.PasswordHashBytes;
    }

    public class HashingOptions
    {
        public const string Section = "Hashing";

        public TargetHashConfigration Target { get; set; } = new TargetHashConfigration();
    }
}
