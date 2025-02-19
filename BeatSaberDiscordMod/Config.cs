using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatSaberDiscordMod
{
    public class BotConfig
    {
        public virtual bool ModEnabled { get; set; } = true;
        public virtual string BotToken { get; set; } = "YOUR_BOT_TOKEN_HERE";
        public virtual bool AutoDownloadMaps { get; set; } = true;
        public virtual string UserList { get; set; } = "Userids seperated by ,";
    }

    public static class Configuration
    {
        public static BotConfig Instance { get; private set; }

        public static void Init(BotConfig config)
        {
            Instance = config;
        }
    }
}
