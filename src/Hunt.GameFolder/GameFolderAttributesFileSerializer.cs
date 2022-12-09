using System.Xml;

namespace Hunt.GameFolder
{
    public static class GameFolderAttributesFileSerializer
    {
        public static async Task<MissionBag> DeserializeAsync(Stream stream, CancellationToken token)
        {
            var missionBagData = await ReadMissionBagDataAsync(stream, token);
            var missionBag = ParseMissionBag(missionBagData);

            return missionBag;
        }

        private static Task<Dictionary<string, string>> ReadMissionBagDataAsync(Stream stream, CancellationToken token)
        {
            var missionBagData = new Dictionary<string, string>();

            using (var xmlReader = XmlReader.Create(stream))
            {
                var attributeExists = xmlReader.ReadToDescendant("Attr");

                if (attributeExists)
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        var key = xmlReader.GetAttribute("name");
                        var keyValue = xmlReader.GetAttribute("value");

                        missionBagData.Add(key, keyValue);

                        var attributeNextExists = xmlReader.ReadToNextSibling("Attr");

                        if (attributeNextExists == false)
                        {
                            break;
                        }
                    }
                }
            }

            return Task.FromResult(missionBagData);
        }

        private static MissionBag ParseMissionBag(IDictionary<string, string> store)
        {
            var missionBagVersion = ParseInteger(store, "MissionBagTeamDetailsVersion");

            if (missionBagVersion != 6)
            {
                throw new InvalidDataException("Attributes version is not supported");
            }

            var missionBag = new MissionBag
            {
                IsHunterDead = ParseBoolean(store, "MissionBagIsHunterDead"),
                IsQuickPlay = ParseBoolean(store, "MissionBagIsQuickPlay"),
                Teams = ParseTeams(store),
                Accolades = ParseAccoladeEntries(store),
                Entries = ParseEntries(store)
            };

            return missionBag;
        }

        private static IEnumerable<Entry> ParseEntries(IDictionary<string, string> store)
        {
            var entryList = new List<Entry>();
            var entryListCount = ParseInteger(store, "MissionBagNumEntries");

            if (entryListCount > 0)
            {
                for (var entryIndex = 0; entryIndex < entryListCount; ++entryIndex)
                {
                    var entry = ParseEntry(store, entryIndex);

                    entryList.Add(entry);
                }
            }

            return entryList;
        }

        private static Entry ParseEntry(IDictionary<string, string> store, int entryIndex)
        {
            var entry = new Entry
            {
                Amount = ParseInteger(store, $"MissionBagEntry_{entryIndex}_amount"),
                Category = ParseString(store, $"MissionBagEntry_{entryIndex}_category"),
                DescriptorName = ParseString(store, $"MissionBagEntry_{entryIndex}_descriptorName"),
                DescriptorScore = ParseInteger(store, $"MissionBagEntry_{entryIndex}_descriptorScore"),
                DescriptorType = ParseInteger(store, $"MissionBagEntry_{entryIndex}_descriptorType"),
                IconPath = ParseString(store, $"MissionBagEntry_{entryIndex}_iconPath"),
                IconPath2 = ParseString(store, $"MissionBagEntry_{entryIndex}_iconPath2"),
                Reward = ParseInteger(store, $"MissionBagEntry_{entryIndex}_reward"),
                RewardSize = ParseInteger(store, $"MissionBagEntry_{entryIndex}_rewardSize"),
                UiNam2 = ParseString(store, $"MissionBagEntry_{entryIndex}_uiNam2"),
                UiName = ParseString(store, $"MissionBagEntry_{entryIndex}_uiName"),
                UiName2 = ParseString(store, $"MissionBagEntry_{entryIndex}_uiName2")
            };

            return entry;
        }

        private static IEnumerable<AccoladeEntry> ParseAccoladeEntries(IDictionary<string, string> store)
        {
            var accoladeEntryList = new List<AccoladeEntry>();
            var accoladeEntryListCount = ParseInteger(store, "MissionBagNumAccolades");

            if (accoladeEntryListCount > 0)
            {
                for (var accoladeEntryIndex = 0; accoladeEntryIndex < accoladeEntryListCount; ++accoladeEntryIndex)
                {
                    var accoladeEntry = ParseAccoladeEntry(store, accoladeEntryIndex);

                    accoladeEntryList.Add(accoladeEntry);
                }
            }

            return accoladeEntryList;
        }

        private static AccoladeEntry ParseAccoladeEntry(IDictionary<string, string> store, int accoladeEntryIndex)
        {
            var accoladeEntry = new AccoladeEntry
            {
                BloodlineXp = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_bloodlineXp"),
                Bounty = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_bounty"),
                Category = ParseString(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_category"),
                EventPoints = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_eventPoints"),
                Gems = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_gems"),
                GeneratedGems = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_generatedGems"),
                Gold = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_gold"),
                Header = ParseString(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_header"),
                Hits = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_hits"),
                HunterPoints = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_hunterPoints"),
                HunterXp = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_hunterXp"),
                IconPath = ParseString(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_iconPath"),
                Weighting = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_weighting"),
                Xp = ParseInteger(store, $"MissionAccoladeEntry_{accoladeEntryIndex}_xp"),
            };

            return accoladeEntry;
        }

        private static IEnumerable<Team> ParseTeams(IDictionary<string, string> store)
        {
            var teamsList = new List<Team>();
            var teamsListCount = ParseInteger(store, "MissionBagNumTeams");

            if (teamsListCount > 0)
            {
                for (var teamIndex = 0; teamIndex < teamsListCount; ++teamIndex)
                {
                    var team = ParseTeam(store, teamIndex);

                    teamsList.Add(team);
                }
            }

            return teamsList;
        }

        private static Team ParseTeam(IDictionary<string, string> store, int teamIndex)
        {
            var team = new Team
            {
                Handicap = ParseInteger(store, $"MissionBagTeam_{teamIndex}_handicap"),
                IsInvite = ParseBoolean(store, $"MissionBagTeam_{teamIndex}_isinvite"),
                Mmr = ParseInteger(store, $"MissionBagTeam_{teamIndex}_mmr"),
                OwnTeam = ParseBoolean(store, $"MissionBagTeam_{teamIndex}_ownteam"),
                Players = ParsePlayers(store, teamIndex)
            };

            return team;
        }

        private static IEnumerable<Player> ParsePlayers(IDictionary<string, string> store, int teamIndex)
        {
            var playersList = new List<Player>();
            var playersListCount = ParseInteger(store, $"MissionBagTeam_{teamIndex}_numplayers");

            if (playersListCount > 0)
            {
                for (var playerIndex = 0; playerIndex < playersListCount; ++playerIndex)
                {
                    var player = ParsePlayer(store, teamIndex, playerIndex);

                    playersList.Add(player);
                }
            }

            return playersList;
        }

        private static Player ParsePlayer(IDictionary<string, string> store, int teamIndex, int playerIndex)
        {
            var player = new Player
            {
                BloodLineName = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_blood_line_name"),
                BountyExtracted = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_bountyextracted"),
                BountyPickedUp = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_bountypickedup"),
                DownedByMe = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_downedbyme"),
                DownedByTeammate = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_downedbyteammate"),
                DownedMe = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_downedme"),
                DownedTeammate = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_downedteammate"),
                HadWellspring = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_hadWellspring"),
                HadBounty = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_hadbounty"),
                IsPartner = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_ispartner"),
                IsSoulSurvivor = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_issoulsurvivor"),
                KilledByMe = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_killedbyme"),
                KilledByTeammate = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_killedbyteammate"),
                KilledMe = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_killedme"),
                KilledTeammate = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_killedteammate"),
                Mmr = ParseInteger(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_mmr"),
                ProfileId = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_profileid"),
                Proximity = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_proximity"),
                ProximityToMe = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_proximitytome"),
                ProximityToTeammate = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_proximitytoteammate"),
                SkillBased = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_skillbased"),
                TeamExtraction = ParseBoolean(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_teamextraction"),
                TooltipDownedByTeammate = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltip_downedbyteammate"),
                TooltipBountyExtracted = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipbountyextracted"),
                TooltipBountyPickedUp = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipbountypickedup"),
                TooltipDownedByMe = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipdownedbyme"),
                TooltipDownedMe = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipdownedme"),
                TooltipDownedTeammate = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipdownedteammate"),
                TooltipKilledByMe = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipkilledbyme"),
                TooltipKilledByTeammate = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipkilledbyteammate"),
                TooltipKilledMe = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipkilledme"),
                TooltipKilledTeammate = ParseString(store, $"MissionBagPlayer_{teamIndex}_{playerIndex}_tooltipkilledteammate"),
            };

            return player;
        }

        private static string ParseString(IDictionary<string, string> store, string key, string defaultValue = null)
        {
            var valueExists = store.TryGetValue(key, out var value);

            if (valueExists == false)
            {
                if (defaultValue != null)
                {
                    return defaultValue;
                }

                throw new InvalidDataException("Attribute doesn't exist");
            }

            return value;
        }

        private static bool ParseBoolean(IDictionary<string, string> store, string key, bool? defaultValue = null)
        {
            var valueExists = store.TryGetValue(key, out var value);

            if (valueExists == false)
            {
                if (defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                throw new InvalidDataException("Attribute doesn't exist");
            }

            return bool.Parse(value);
        }

        private static int ParseInteger(IDictionary<string, string> store, string key, int? defaultValue = null)
        {
            var valueExists = store.TryGetValue(key, out var value);

            if (valueExists == false)
            {
                if (defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                throw new InvalidDataException("Attribute doesn't exist");
            }

            return int.Parse(value);
        }
    }
}