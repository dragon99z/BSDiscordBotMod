using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using System.Linq;
using UnityEngine.SceneManagement;
using BeatSaverSharp;
using BeatSaberDiscordMod.UI;
using Logger = IPA.Logging.Logger;
using DiscordUnity;
using DiscordUnity.API;
using DiscordUnity.State;
using UnityEngine.PlayerLoop;
using Zenject;
using HMUI;
using System.Collections;

namespace BeatSaberDiscordMod
{
    public class BeatSaberDiscordModController : MonoBehaviour
    {
        public static BeatSaberDiscordModController Instance;
        public static List<string> mapQueue = new List<string>();
        public static bool inDownloader = false;
        private static BeatSaberDiscordModFlowCoordinator _flowCoordinator;
        private static DownloadFlowCoordinator _downloadFlowCoordinator;
        private static BotConfig _config;

        [Inject] private readonly MainFlowCoordinator _mainFlow; // Inject the main menu


        public void Awake()
        {
            if (Instance != null)
            {
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            _config = Configuration.Instance;
        }

        public static void Init()
        {
            MenuButtons.Instance.RegisterButton(new MenuButton("BeatSaberDiscordMod", "Setup Beat Saber Discord Bot", OpenSettingsPrompt));
            MenuButtons.Instance.RegisterButton(new MenuButton("BS DC Downloads", "Stil wanna download?", OpenDownloadPrompt));
            if (_config.ModEnabled && (_config.BotToken != "" || _config.BotToken != "YOUR_BOT_TOKEN_HERE"))
            {
                Task.Run(StartDiscordBot);
            }
            //GameplaySetup.Instance.AddTab("BeatSaberDiscordMod", "BeatSaberDiscordMod.UI.BeatSaberDiscordModViewController.bsml",);
        }

        private static void OpenSettingsPrompt()
        {
            if (_flowCoordinator == null)
                _flowCoordinator = BeatSaberUI.CreateFlowCoordinator<BeatSaberDiscordModFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_flowCoordinator);
        }

        private static void OpenDownloadPrompt()
        {
            if(mapQueue.Count > 0)
            {
                if (_downloadFlowCoordinator == null)
                    _downloadFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<DownloadFlowCoordinator>();
                BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_downloadFlowCoordinator);
                inDownloader = true;
            }
        }

        private static async void StartDiscordBot()
        {
            try
            {
                await DiscordAPI.StartWithBot(_config.BotToken);
                DiscordAPI.RegisterEventsHandler(new Handler());
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BeatSaberDiscordMod] Failed to start Discord bot: {ex}");
            }
        }


        /*
        private async Task StartDiscordBot()
        {
            logger.Info("Starting Bot");
            DiscordConfiguration discordConfiguration = new DiscordConfiguration();
            discordConfiguration.Token = _config.BotToken;
            discordConfiguration.TokenType = TokenType.Bot;
            discordConfiguration.Intents = DiscordIntents.Guilds | DiscordIntents.GuildMembers | DiscordIntents.DirectMessages;
            _discordClient = new DiscordClient(discordConfiguration);

            var slashCommands = _discordClient.UseSlashCommands();
            slashCommands.RegisterCommands<BeatSaverCommands>();

            await _discordClient.ConnectAsync(null,UserStatus.Online);
            logger.Info("Bot Started");
            await Task.Delay(-1);
        }*/


        private IEnumerator coroutine = null;

        private IEnumerator openDownloadMenu()
        {
            yield return new WaitForSeconds(1f);
            if (_downloadFlowCoordinator == null)
                _downloadFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<DownloadFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_downloadFlowCoordinator);
            inDownloader = true;
            this.coroutine = null;
        }

        public void Update()
        {
            DiscordAPI.Update();
            NoTransitionsButton backButton = GameObject.FindObjectsOfType<NoTransitionsButton>().FirstOrDefault(b => b.gameObject.name == "BackButton");
            if (SceneManager.GetActiveScene().name == "MainMenu" && (backButton == null || !backButton.gameObject.activeInHierarchy) && mapQueue.Count > 0 && !inDownloader)
            {
                if (this.coroutine == null)
                {
                    this.coroutine = openDownloadMenu();
                    StartCoroutine(this.coroutine);
                }
                
            }
        }
    }


    class Handler : IDiscordMessageEvents
    {

        Logger logger = Plugin.Log;

        public void OnMessageAllReactionsRemoved(DiscordMessage message, DiscordReaction reaction)
        {
            
        }

        public void OnMessageCreated(DiscordMessage message)
        {
            logger.Info("MSG resived!");
            List<string> userList;
            if (Configuration.Instance.UserList.Contains(","))
                userList = Configuration.Instance.UserList.Split(',').ToList();
            else
            {
                userList = new List<string>();
                userList.Add(Configuration.Instance.UserList);
            }
            logger.Info("Userlist :" + Configuration.Instance.UserList);
            if (message.Content.StartsWith("!") && userList.Contains(message.Author.Id))
            {
                logger.Info("Cmd resived!");
                string msg = message.Content.Substring(1);
                string[] msgarry = msg.Split(' ');
                string command = msgarry[0];
                string[] args = msgarry.Where(w => w != msgarry[0]).ToArray();

                logger.Info("Cmd :" + command);
                logger.Info("args :" + args);

                switch (command)
                {
                    case "download":
                        if (args[0].Contains("beatsaver.com"))
                        {
                            logger.Info("download resived!");
                            BeatSaberDiscordModController.mapQueue.Add(args[0]);
                            DownloadViewController.StaticRefreshUI();
                            message.Channel.CreateMessage("Map '" + args[0] +"' has been added!", null, null, null, null, null, null);
                        }
                        else
                            message.Channel.CreateMessage("Please send a beatsaver.com link!", null, null, null, null, null, null);
                        break;
                }
            }
        }

        public void OnMessageDeleted(DiscordMessage message)
        {
            
        }

        public void OnMessageDeletedBulk(string[] messageIds)
        {
            
        }

        public void OnMessageEmojiReactionRemoved(DiscordMessage message, DiscordReaction reaction)
        {
            
        }

        public void OnMessageReactionAdded(DiscordMessage message, DiscordReaction reaction)
        {
            
        }

        public void OnMessageReactionRemoved(DiscordMessage message, DiscordReaction reaction)
        {
            
        }

        public void OnMessageUpdated(DiscordMessage message)
        {
            
        }

        public void OnServerBan(DiscordServer server, DiscordUser user)
        {

        }

        public void OnServerEmojisUpdated(DiscordServer server, DiscordEmoji[] emojis)
        {

        }

        public void OnServerJoined(DiscordServer server)
        {
            //server.Channels.Values.FirstOrDefault(x => x.Type == DiscordUnity.Models.ChannelType.GUILD_TEXT)?.CreateMessage("Hello World!", null, null, null, null, null, null);
        }

        public void OnServerLeft(DiscordServer server)
        {

        }

        public void OnServerMemberJoined(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerMemberLeft(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerMembersChunk(DiscordServer server, DiscordServerMember[] members, string[] notFound, DiscordPresence[] presences)
        {

        }

        public void OnServerMemberUpdated(DiscordServer server, DiscordServerMember member)
        {

        }

        public void OnServerRoleCreated(DiscordServer server, DiscordRole role)
        {

        }

        public void OnServerRoleRemove(DiscordServer server, DiscordRole role)
        {

        }

        public void OnServerRoleUpdated(DiscordServer server, DiscordRole role)
        {

        }

        public void OnServerUnban(DiscordServer server, DiscordUser user)
        {

        }

        public void OnServerUpdated(DiscordServer server)
        {

        }
    }
}
