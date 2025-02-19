using BeatSaberDiscordMod.UI;
using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberDiscordMod
{
    internal class DownloadFlowCoordinator : FlowCoordinator
    {

        private DownloadViewController _settingsView;

        public void Awake()
        {
            if (_settingsView == null)
                _settingsView = BeatSaberUI.CreateViewController<DownloadViewController>();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
                return;

            SetTitle("Download?");
            showBackButton = true;
            ProvideInitialViewControllers(_settingsView);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            var mainFlow = BeatSaberUI.MainFlowCoordinator;
            mainFlow.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal);
        }

    }
}
