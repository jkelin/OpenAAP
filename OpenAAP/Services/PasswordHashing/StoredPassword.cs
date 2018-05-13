using OpenAAP.Options;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    [ProtoContract]
    public class StoredPassword
    {
        [ProtoMember(1)]
        public byte[] Hash { get; set; }

        [ProtoMember(2)]
        public byte[] Salt { get; set; }

        [ProtoMember(3)]
        public TargetHashConfigration Options { get; set; }

        public static StoredPassword From(byte[] hash, byte[] salt, TargetHashConfigration opts)
        {
            return new StoredPassword
            {
                Hash = hash,
                Salt = salt,
                Options = opts,
            };
        }

        public static StoredPassword Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<StoredPassword>(stream);
            }
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }
    }
}
