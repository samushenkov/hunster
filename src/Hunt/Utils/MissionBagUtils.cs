using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hunt.Entity;

namespace Hunt.Utils
{
    public static class MissionBagUtils
    {
        private static JsonSerializerOptions _serializerOptions;

        static MissionBagUtils()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
        }

        public static string GetMissionHash(MissionBag missionBag)
        {
            var data = JsonSerializer.Serialize(missionBag, _serializerOptions);

            var bytes = Encoding.UTF8.GetBytes(data);
            var bytesHash = MD5.HashData(bytes);

            return string.Create(bytesHash.Length * 2, bytesHash, static (chars, bytes) =>
            {
                for (var byteIndex = 0; byteIndex < bytes.Length; ++byteIndex)
                {
                    var chr = bytes[byteIndex];
                    var chrIndex = byteIndex * 2;

                    chars[chrIndex] = ToCharLower(chr >> 4);
                    chars[chrIndex + 1] = ToCharLower(chr);
                }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char ToCharLower(int value)
        {
            value &= 0xF;

            if (value >= 10)
            {
                return (char)(value - 10 + 'a');
            }

            return (char)(value + '0');
        }
    }
}
