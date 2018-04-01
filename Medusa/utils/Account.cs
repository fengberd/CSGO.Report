using System;
using System.Collections.Generic;

using SteamAuth;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.Internal;
using SteamKit2.GC.CSGO.Internal;

using BakaServer;

namespace Medusa.utils
{
    public class Account
    {
        public const int APPID_CSGO = 730;

        public static Config LoginKeys = new Config("loginKeys.ini");

        public int FailReportCounter = -1;
        public bool Connected => steamClient.IsConnected;
        public bool DelayedActionsEmpty => actions.Count == 0;
        public bool LoggedIn = false, ProcessingReport = false, WaitingForCode = false, GameRunning = false, GameInitalized = false;
        public string AuthCode = null, TwoFactorCode = null;

        public bool Protected = false;
        public string Username, Password, SharedSecret;

        public string PREFIX = "";

        public SentryFile sentry;

        private SteamUser steamUser;
        private SteamClient steamClient;
        private SteamFriends steamFriends;
        private SteamGameCoordinator steamGameCoordinator;

        private CallbackManager callbackManager;

        private Queue<ReportInfo> reportQueue = new Queue<ReportInfo>();
        private List<AccountDelayAction> actions = new List<AccountDelayAction>();

        public Account(string Username,string Password,bool Protected = false,string SharedSecret = "")
        {
            this.Username = Username;
            this.Password = Password;
            this.Protected = Protected;
            this.SharedSecret = SharedSecret;
            this.PREFIX = "[" + Username + "] ";

            sentry = new SentryFile(Username);
            steamClient = new SteamClient(SteamConfiguration.Create((builder) => builder.WithConnectionTimeout(TimeSpan.FromSeconds(20))));
            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();
            steamGameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            callbackManager = new CallbackManager(steamClient);
            callbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            callbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            callbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuth);
            callbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            callbackManager.Subscribe<SteamUser.LoginKeyCallback>((callback) =>
            {
                LoginKeys[Username] = callback.LoginKey;
                LoginKeys.Save();
            });

            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
        }

        public void Tick(long Tick)
        {
            callbackManager.RunCallbacks();
            if(Tick % 20 == 0)
            {
                lock(actions)
                {
                    var to_remove = new List<AccountDelayAction>();
                    foreach(var action in actions)
                    {
                        if(--action.SecondsRemain <= 0)
                        {
                            action.Action.Invoke();
                            to_remove.Add(action);
                        }
                    }
                    to_remove.ForEach((a) => actions.Remove(a));
                }
            }
            if(!LoggedIn)
            {
                return;
            }
            if(!ProcessingReport)
            {
                if(reportQueue.Count != 0)
                {
                    if(!GameRunning)
                    {
                        StartGame();
                    }
                    if(GameInitalized)
                    {
                        var report = reportQueue.Peek();
                        steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientReportPlayer>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientReportPlayer)
                        {
                            Body =
                            {
                                account_id = report.SteamID.AccountID,
                                match_id = report.MatchID,
                                rpt_aimbot = Convert.ToUInt32(report.AimHacking),
                                rpt_wallhack = Convert.ToUInt32(report.WallHacking),
                                rpt_speedhack = Convert.ToUInt32(report.OtherHacking),
                                rpt_teamharm = Convert.ToUInt32(report.Griefing),
                                rpt_textabuse = Convert.ToUInt32(report.AbusiveText),
                                rpt_voiceabuse = Convert.ToUInt32(report.AbusiveVoice)
                            }
                        },APPID_CSGO);
                        FailReportCounter = 30 * 20; // Fail the action if no response in 30s.
                        ProcessingReport = true;
                    }
                }
                else if(Program.IsOnlineTimeRange())
                {
                    if(!GameRunning)
                    {
                        StartGame();
                    }
                }
                else if(GameRunning)
                {
                    StopGame();
                }
            }
            else if(FailReportCounter > -1 && --FailReportCounter <= 0)
            {
                Logger.Error(PREFIX + "No report response recieved.Dropping the failed report info.");
                FailReportCounter = -1;
                ProcessingReport = false;
                reportQueue.Dequeue();
            }
        }

        public bool Connect()
        {
            if(WaitingForCode && AuthCode == null && TwoFactorCode == null)
            {
                return false;
            }
            LoggedIn = false;
            Logger.Debug(PREFIX + "Connecting...");
            steamClient.Connect();
            return true;
        }

        public bool Disconnect()
        {
            if(!LoggedIn)
            {
                return false;
            }
            steamClient.Disconnect();
            return true;
        }

        public void QueueReport(ReportInfo info)
        {
            reportQueue.Enqueue(info);
        }

        public void AddDelayAction(int delay,Action action)
        {
            lock(actions)
            {
                actions.Add(new AccountDelayAction()
                {
                    Action = action,
                    SecondsRemain = delay
                });
            }
        }

        protected void UpdateGameStatus()
        {
            var clientGamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            if(GameRunning)
            {
                clientGamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed()
                {
                    game_id = APPID_CSGO
                });
                AddDelayAction(2,() => steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello),APPID_CSGO));
            }
            steamClient.Send(clientGamesPlayed);
        }

        protected void StartGame()
        {
            if(GameRunning)
            {
                return;
            }
            var clientGamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            clientGamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed()
            {
                game_id = APPID_CSGO
            });
            GameRunning = true;
            GameInitalized = false;
            steamClient.Send(clientGamesPlayed);
            AddDelayAction(2,() => steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello),APPID_CSGO));
        }

        protected void StopGame()
        {
            if(!GameRunning)
            {
                return;
            }
            GameRunning = GameInitalized = false;
            steamClient.Send(new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed));
        }

        #region Steam Callbacks

        protected void OnUpdateMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            sentry.Write(callback.Offset,callback.Data,callback.BytesToWrite);
            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,
                FileName = callback.FileName,
                BytesWritten = callback.BytesToWrite,
                FileSize = sentry.Length,
                Offset = callback.Offset,
                Result = EResult.OK,
                LastError = 0,
                OneTimePassword = callback.OneTimePassword,
                SentryFileHash = sentry.Hash
            });
        }

        protected void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Logger.Debug(PREFIX + "Connected to steam.");
            var random = new Random();
            steamUser.LogOn(new SteamUser.LogOnDetails()
            {
                Username = Username,
                Password = LoginKeys.ContainsKey(Username) ? null : Password,
                LoginID = ((uint)random.Next(1 << 30) << 2) | (uint)random.Next(1 << 2),
                LoginKey = LoginKeys.ContainsKey(Username) ? LoginKeys[Username] : null,
                ShouldRememberPassword = true,
                SentryFileHash = sentry.Exists ? sentry.Hash : null,
                AuthCode = AuthCode,
                TwoFactorCode = TwoFactorCode,
                ClientOSType = EOSType.Windows10,
                ClientLanguage = "en-US"
            });
        }

        protected void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Logger.Debug(PREFIX + "Disconnected");
            if(!callback.UserInitiated && LoggedIn)
            {
                Logger.Info(PREFIX + "Disconnected from steam by accident,retrying in 5 seconds.");
                AccountManager.DelayedLoginQueue.Enqueue(this);
            }
            LoggedIn = false;
            ProcessingReport = WaitingForCode = GameRunning = GameInitalized = false;
        }

        protected void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            switch(callback.Result)
            {
            case EResult.OK:
                steamFriends.SetPersonaState(EPersonaState.Online);
                Logger.Info(PREFIX + "Successfully logged in to steam.");
                LoggedIn = true;
                UpdateGameStatus();
                break;
            case EResult.AccountLogonDenied:
            case EResult.AccountLoginDeniedNeedTwoFactor:
                if(!Protected)
                {
                    Logger.Error(PREFIX + "Requires steam token to log in.");
                }
                else if(callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
                {
                    if(SharedSecret == "")
                    {
                        Logger.Error(PREFIX + "Requires shared secret to log in.");
                        break;
                    }
                    Logger.Info(PREFIX + "Generating two factor auth code...");
                    AuthCode = null;
                    TwoFactorCode = new SteamGuardAccount()
                    {
                        SharedSecret = SharedSecret
                    }.GenerateSteamGuardCode();
                    Connect();
                }
                else if(!Program.config.GetBool("MailClientEnabled"))
                {
                    Logger.Error(PREFIX + "Requires steam token to log in,please configure mail client.");
                }
                else
                {
                    Logger.Info(PREFIX + "Requires steam token to log in,waiting for mail...");
                    WaitingForCode = true;
                    TwoFactorCode = AuthCode = null;
                }
                break;
            case EResult.InvalidPassword:
                if(LoginKeys.ContainsKey(Username))
                {
                    Logger.Error(PREFIX + "Login Key invalid,retrying...");
                    LoginKeys.Remove(Username);
                    LoginKeys.Save();
                    steamClient.Connect();
                }
                else
                {
                    Logger.Error(PREFIX + "Password incorrect.");
                }
                break;
            case EResult.RateLimitExceeded:
                Logger.Error(PREFIX + "Steam Rate Limit has been reached.Retrying in 30 minutes...");
                AddDelayAction(1200,() => Connect());
                break;
            case EResult.AccountDisabled:
                Logger.Error(PREFIX + "has been permanently disabled by the Steam network.");
                break;
            default:
                Logger.Error(PREFIX + "Unable to login: " + callback.Result + "(" + callback.ExtendedResult + ").Retrying in 10 seconds...");
                AddDelayAction(10,() => Connect());
                break;
            }
        }

        protected void OnGCMessage(SteamGameCoordinator.MessageCallback callback)
        {
            if(!LoggedIn)
            {
                return;
            }
            var msg = callback.Message;
            switch(callback.EMsg)
            {
            case (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome:
                {
                    var response = new ClientGCMsgProtobuf<CMsgClientWelcome>(msg);
                    Logger.Info(PREFIX + "Connected to CS:GO " + response.Body.location.country + " server.");
                    steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello),APPID_CSGO);
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello:
                {
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg);
                    if(response.Body.penalty_reasonSpecified)
                    {
                        LoggedIn = false;
                        switch(response.Body.penalty_reason)
                        {
                        case 10:
                            Logger.Error(PREFIX + "Account has been convicted by Overwatch as majorly disruptive.");
                            return;
                        case 11:
                            Logger.Error(PREFIX + "Account has been convicted by Overwatch as minorly disruptive.");
                            return;
                        case 14:
                            Logger.Error(PREFIX + "Account is permanently untrusted.");
                            return;
                        default:
                            if(response.Body.penalty_secondsSpecified)
                            {
                                var penalty = TimeSpan.FromSeconds(response.Body.penalty_seconds);
                                if(penalty.Seconds <= 604800)
                                {
                                    string timeString;
                                    if(penalty.Minutes >= 60)
                                    {
                                        timeString = penalty.Hours + " Hours";
                                    }
                                    else
                                    {
                                        timeString = penalty.Minutes + " Minutes";
                                    }

                                    Logger.Warning(PREFIX + "Account has received a Matchmaking cooldown.Retrying in " + penalty.Seconds + " seconds.");
                                    steamClient.Disconnect();
                                    AddDelayAction(penalty.Seconds,() => Connect());
                                    return;
                                }
                            }
                            Logger.Error(PREFIX + "Account has been permanently banned from CS:GO.");
                            return;
                        }
                    }
                    else if(response.Body.vac_bannedSpecified && response.Body.vac_banned == 2 && !response.Body.penalty_secondsSpecified)
                    {
                        LoggedIn = false;
                        Logger.Error(PREFIX + "Account has been banned by VAC,VOLVO ARE YOU KIDDING ME????");
                        return;
                    }
                    GameInitalized = true;
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientReportResponse:
                {
                    var report = reportQueue.Dequeue();
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientReportResponse>(msg);
                    MedusaWebServer.addReportLog(new Dictionary<string,string>()
                    {
                        { "username",Username },
                        { "steamid", report.SteamID.ConvertToUInt64().ToString() },
                        { "matchid", report.MatchID.ToString() },
                        { "reportid", response.Body.confirmation_id.ToString() },
                        { "time", Utils.Time().ToString() },
                    });
                    FailReportCounter = -1;
                    ProcessingReport = false;
                    Logger.Info(PREFIX + "Successfully reported " + report.SteamID + ",Confirmation ID:" + response.Body.confirmation_id);
                }
                break;
            }
        }

        #endregion
    }
}
