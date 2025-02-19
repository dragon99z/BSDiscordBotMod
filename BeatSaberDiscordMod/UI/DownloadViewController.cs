using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using HMUI;
using IPA.Utilities.Async;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;
using static AutoRecord;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;


namespace BeatSaberDiscordMod.UI
{
    [HotReload(RelativePathToLayout = @"DownloadViewController.bsml")]
    [ViewDefinition("BeatSaberDiscordMod.UI.DownloadViewController.bsml")]
    internal class DownloadViewController : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        BeatSaver beatSaver = new BeatSaver(new BeatSaverOptions("BeatSaberDiscordMod", "0.0.1"));

        [UIComponent("mapList")]
        private TextMeshProUGUI textComponent;

        public static DownloadViewController Instance { get; private set; } // Static reference

        private void Awake()
        {
            Instance = this; // Assign the instance when the UI loads
        }

        private bool parsed = false;

        [UIAction("#post-parse")]
        void Parsed()
        {
            parsed = true;
            Instance.StartCoroutine(Instance.DelayedUIUpdate());
        }

        public static void StaticRefreshUI()
        {
            if (Instance != null && Instance.parsed)
            {
                Instance.StartCoroutine(Instance.DelayedUIUpdate());
            }
        }

        private bool _isUpdating = false;

        private IEnumerator DelayedUIUpdate()
        {
            if (_isUpdating) yield break; // Prevent multiple updates at once
            _isUpdating = true;

            yield return new WaitForSeconds(0.1f); // ✅ Delay UI updates slightly

            string text = "";
            foreach (var url in BeatSaberDiscordModController.mapQueue)
            {
                WebClient x = new WebClient();
                string source = x.DownloadString(url);
                string title = Regex.Match(source, @"\<meta name=\""og:title\"" content=""\s*(?<Title>[\s\S]*?)\""\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                Plugin.Log.Info(title);
                text += title + "\n";
            }
            textComponent.text = text;
            NotifyPropertyChanged(nameof(textComponent));
            _isUpdating = false;
        }

        /*
        [UIAction("#post-parse")]
        void Parsed()
        {
            verticalLayoutTag.Initialize();
            IList<object> list = new List<object>();
            foreach (var map in BeatSaberDiscordModController.mapQueue) 
            {
                TextMeshProUGUI label = new TextMeshProUGUI();
                label.color = new UnityEngine.Color(
                    255,
                    255,
                    255,
                    0.8f
                );
                label.text = beatSaver.Beatmap(urlToKey(map)).Result.Name;
                list.Add(label);
            }
            mapList.Data = (System.Collections.IList)list;
            mapList.TableView.ReloadData();
            mapList.TableView.ClearSelection();
        }*/

        private IEnumerator ReloadSongList()
        {
            yield return new WaitForSeconds(1f);
            SongCore.Loader.Instance.RefreshSongs();
        }

        

        private IEnumerator goBack()
        {
            yield return new WaitForSeconds(1f);
            NoTransitionsButton backButton = GameObject.FindObjectsOfType<NoTransitionsButton>().FirstOrDefault(b => b.gameObject.name == "BackButton");
            if (backButton != null)
            {
                backButton.onClick.Invoke();
            }
        }

        [UIAction("download-maps")]
        public void DownloadMaps()
        {
            foreach (var url in BeatSaberDiscordModController.mapQueue)
            {
                
                CancellationToken cancellationToken = new CancellationToken();
                Task.Run(async () =>
                {
                    await BeatSaverDownloader.Misc.SongDownloader.Instance.DownloadSong(beatSaver.Beatmap(urlToKey(url)).Result, cancellationToken);
                }).ConfigureAwait(true);
            }
            BeatSaberDiscordModController.mapQueue.Clear();
            textComponent.text = "";
            BeatSaberDiscordModController.inDownloader = false;
            parsed = false;
            StartCoroutine(ReloadSongList());
            StartCoroutine(goBack());
        }

        public string urlToKey(string url)
        {
            string newUrl = url.ToString();
            if (newUrl.EndsWith("/"))
                newUrl = newUrl.Remove(newUrl.Length - 1);
            newUrl = newUrl.Remove(0, newUrl.LastIndexOf('/'));
            return newUrl;
        }
    }
}
