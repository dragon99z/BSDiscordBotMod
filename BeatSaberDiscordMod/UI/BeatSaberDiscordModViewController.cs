using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BeatSaberDiscordMod.UI
{
    [HotReload(RelativePathToLayout = @"BeatSaberDiscordModViewController.bsml")]
    [ViewDefinition("BeatSaberDiscordMod.UI.BeatSaberDiscordModViewController.bsml")]
    internal class BeatSaberDiscordModViewController : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        [UIValue("mod-enabled")]
        protected bool ModEnabled
        {
            get => Configuration.Instance.ModEnabled;
            set => Configuration.Instance.ModEnabled = value;
        }

        [UIValue("bot-token")]
        protected string BotToken
        {
            get => Configuration.Instance.BotToken;
            set => Configuration.Instance.BotToken = value;
        }

        [UIValue("auto-download-maps")]
        protected bool AutoDownloadMaps
        {
            get => Configuration.Instance.AutoDownloadMaps;
            set => Configuration.Instance.AutoDownloadMaps = value;
        }

        [UIValue("user-list")]
        protected string UserList
        {
            get => Configuration.Instance.UserList;
            set => Configuration.Instance.UserList = value;
        }
    }
}
