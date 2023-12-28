using Common.String;
using Hunt.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            return StringUtils.ToHex(bytesHash);
        }

        public static Player GetOwnPlayer(MissionBag missionBag)
        {
            foreach (var team in missionBag.Teams)
            {
                if (team.OwnTeam == false)
                {
                    continue;
                }

                foreach (var teamPlayer in team.Players)
                {
                    if (teamPlayer.ProximityToMe && 
                        teamPlayer.IsPartner == false)
                    {
                        return teamPlayer;
                    }
                }

                break;
            }

            return null;
        }

        public static Player GetPlayerById(MissionBag missionBag, string profileId)
        {
            foreach (var team in missionBag.Teams)
            {
                foreach (var teamPlayer in team.Players)
                {
                    if (teamPlayer.ProfileId == profileId)
                    {
                        return teamPlayer;
                    }
                }
            }

            return null;
        }
    }
}
