using System;
using System.Collections.Generic;

using SteamAuth;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.Internal;
using SteamKit2.GC.CSGO.Internal;

using BakaServer;

using Medusa.utils.actions;

namespace Medusa.utils
{
    public class Account
    {
        public const int APPID_CSGO = 730;

        public static Config LoginKeys = new Config("loginKeys.ini");

        public string PREFIX = "";

        public bool Protected = false;
        public string Username, Password, SharedSecret;

        public bool LoggedIn = false, WaitingForCode = false, Disabled = false, GameRunning = false, GameInitalized = false;
        public string AuthCode = null, TwoFactorCode = null;

        public bool ProcessingAction = false;
        public int FailActionCounter = -1;

        public bool Connected => !Disabled && steamClient.IsConnected;
        public bool DelayedActionsEmpty => actions.Count == 0;

        public SentryFile sentry;

        private SteamUser steamUser;
        private SteamClient steamClient;
        private SteamFriends steamFriends;
        private SteamGameCoordinator steamGameCoordinator;

        private CallbackManager callbackManager;

        private Queue<SendInfo> sendQueue = new Queue<SendInfo>();
        private Queue<ActionInfo> actionQueue = new Queue<ActionInfo>();
        private List<AccountDelayedAction> actions = new List<AccountDelayedAction>();

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

        public bool ProcessSendQueue()
        {
            lock(sendQueue)
            {
                if(sendQueue.Count > 0)
                {
                    var item = sendQueue.Dequeue();
                    switch(item.Type)
                    {
                    case SendInfo.SendType.SteamClient:
                        steamClient.Send((IClientMsg)item.Packet);
                        break;
                    case SendInfo.SendType.SteamGameCoordinator:
                        steamGameCoordinator.Send((IClientGCMsg)item.Packet,APPID_CSGO);
                        break;
                    }
                    return true;
                }
            }
            return false;
        }

        public void Tick(long Tick)
        {
            callbackManager.RunCallbacks();
            if(Tick % 20 == 0)
            {
                lock(actions)
                {
                    var to_remove = new List<AccountDelayedAction>();
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
            if(!ProcessingAction)
            {
                lock(actionQueue)
                {
                    if(actionQueue.Count != 0)
                    {
                        if(!GameRunning)
                        {
                            StartGame();
                        }
                        if(GameInitalized)
                        {
                            var action = actionQueue.Peek();
                            lock(sendQueue)
                            {
                                if(action is ReportInfo)
                                {
                                    var report = action as ReportInfo;
                                    sendQueue.Enqueue(new SendInfo()
                                    {
                                        Type = SendInfo.SendType.SteamGameCoordinator,
                                        Packet = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientReportPlayer>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientReportPlayer)
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
                                        }
                                    });
                                }
                                else if(action is CommendInfo)
                                {
                                    var commend = action as CommendInfo;
                                    sendQueue.Enqueue(new SendInfo()
                                    {
                                        Type = SendInfo.SendType.SteamGameCoordinator,
                                        Packet = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientCommendPlayer>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientCommendPlayer)
                                        {
                                            Body =
                                        {
                                            account_id = commend.SteamID.AccountID,
                                            match_id = commend.MatchID,
                                            commendation = new PlayerCommendationInfo
                                            {
                                                cmd_friendly = Convert.ToUInt32(commend.Friendly),
                                                cmd_teaching = Convert.ToUInt32(commend.GoodTeacher),
                                                cmd_leader = Convert.ToUInt32( commend.GoodLeader)
                                            },
                                            tokens = 0
                                        }
                                        }
                                    });
                                }
                                else if(action is GetLiveGameInfo)
                                {
                                    sendQueue.Enqueue(new SendInfo()
                                    {
                                        Type = SendInfo.SendType.SteamGameCoordinator,
                                        Packet = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestLiveGameForUser>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestLiveGameForUser)
                                        {
                                            Body =
                                        {
                                            accountid = action.SteamID.AccountID
                                        }
                                        }
                                    });
                                }
                            }
                            FailActionCounter = 60 * 20;
                            ProcessingAction = true;
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
            }
            else if(FailActionCounter > -1 && --FailActionCounter <= 0)
            {
                Logger.Error(PREFIX + "No action response recieved.Dropping the failed action info.");
                FailActionCounter = -1;
                ProcessingAction = false;
                lock(actionQueue)
                {
                    try
                    {
                        actionQueue.Dequeue();
                    }
                    catch(Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }
        }


        public bool Connect()
        {
            if(Disabled || (WaitingForCode && AuthCode == null && TwoFactorCode == null))
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

        public void QueueAction(ActionInfo info) => actionQueue.Enqueue(info);

        public void AddDelayedAction(uint delay,Action action)
        {
            lock(actions)
            {
                actions.Add(new AccountDelayedAction()
                {
                    Action = action,
                    SecondsRemain = delay
                });
            }
        }

        protected void SendGameStatus()
        {
            lock(sendQueue)
            {
                var clientGamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
                if(GameRunning)
                {
                    clientGamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed()
                    {
                        game_id = APPID_CSGO
                    });
                    AddDelayedAction(2,() => sendQueue.Enqueue(new SendInfo()
                    {
                        Type = SendInfo.SendType.SteamGameCoordinator,
                        Packet = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello)
                    }));
                }
                sendQueue.Enqueue(new SendInfo()
                {
                    Type = SendInfo.SendType.SteamClient,
                    Packet = clientGamesPlayed
                });
            }
        }

        protected void StartGame()
        {
            if(GameRunning)
            {
                return;
            }
            GameRunning = true;
            GameInitalized = false;
            SendGameStatus();
        }

        protected void StopGame()
        {
            if(!GameRunning)
            {
                return;
            }
            GameRunning = GameInitalized = false;
            SendGameStatus();
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
            WaitingForCode = false;
            AuthCode = TwoFactorCode = null;
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
            ProcessingAction = GameRunning = GameInitalized = false;
        }

        protected void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            switch(callback.Result)
            {
            case EResult.OK:
                steamFriends.SetPersonaState(EPersonaState.Online);
                Logger.Info(PREFIX + "Successfully logged in to steam.");
                LoggedIn = true;
                GameInitalized = false;
                if(GameRunning)
                {
                    SendGameStatus();
                }
                break;
            case EResult.AccountLogonDenied:
            case EResult.AccountLoginDeniedNeedTwoFactor:
            case EResult.TwoFactorCodeMismatch:
                if(!Protected)
                {
                    Logger.Error(PREFIX + "Requires steam token to log in.");
                }
                else if(callback.Result == EResult.TwoFactorCodeMismatch)
                {
                    AddDelayedAction(30,() => Connect());
                    Logger.Error(PREFIX + "Two factor code mismatch.Retrying in 30 seconds...");
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
                    Disabled = true;
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
                    Disabled = true;
                    Logger.Error(PREFIX + "Password incorrect.");
                }
                break;
            case EResult.RateLimitExceeded:
                Logger.Error(PREFIX + "Steam Rate Limit has been reached.Retrying in 30 minutes...");
                AddDelayedAction(1200,() => Connect());
                break;
            case EResult.AccountDisabled:
                Disabled = true;
                Logger.Error(PREFIX + "has been permanently disabled by the Steam network.");
                break;
            default:
                Logger.Error(PREFIX + "Unable to login: " + callback.Result + "(" + callback.ExtendedResult + ").Retrying in 10 seconds...");
                AddDelayedAction(10,() => Connect());
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
                            Disabled = true;
                            Logger.Error(PREFIX + "Account has been convicted by Overwatch as majorly disruptive.");
                            return;
                        case 11:
                            Disabled = true;
                            Logger.Error(PREFIX + "Account has been convicted by Overwatch as minorly disruptive.");
                            return;
                        case 14:
                            Disabled = true;
                            Logger.Error(PREFIX + "Account is permanently untrusted.");
                            return;
                        default:
                            if(response.Body.penalty_secondsSpecified)
                            {
                                Logger.Warning(PREFIX + "Account has received a Matchmaking cooldown.Retrying in " + response.Body.penalty_seconds + " seconds.");
                                steamClient.Disconnect();
                                AddDelayedAction(response.Body.penalty_seconds,() => Connect());
                                return;
                            }
                            Disabled = true;
                            Logger.Error(PREFIX + "Account has been permanently banned from CS:GO.");
                            return;
                        }
                    }
                    else if(response.Body.vac_bannedSpecified && response.Body.vac_banned == 2 && !response.Body.penalty_secondsSpecified)
                    {
                        LoggedIn = false;
                        Disabled = true;
                        Logger.Error(PREFIX + "Account has been banned by VAC.");
                        return;
                    }
                    GameInitalized = true;
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientReportResponse:
                {
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientReportResponse>(msg);
                    ReportInfo report;
                    lock(actionQueue)
                    {
                        report = actionQueue.Peek() as ReportInfo;
                        if(report == null)
                        {
                            // IDK why we'll get a ClientReportResponse instead of ClientCommendPlayerQueryResponse
                            // CSGO things bro ;D
                            var commend = actionQueue.Peek() as CommendInfo;
                            if(commend == null)
                            {
                                Logger.Warning(PREFIX + "Something wrong happened,maybe we just failed a report and recieved it's response.");
                                break;
                            }
                            if(response.Body.account_idSpecified && commend.SteamID.AccountID != response.Body.account_id)
                            {
                                Logger.Warning(PREFIX + "Something wrong happened,maybe we just failed a commend and recieved it's response.");
                                break;
                            }
                            actionQueue.Dequeue();
                            MedusaWebServer.addLog(new Dictionary<string,object>()
                        {
                            { "type", "commend" },
                            { "username", Username },
                            { "steamid", commend.SteamID.ConvertToUInt64().ToString() },
                            { "flags", commend.Flags },
                            { "time", Utils.Time() },
                        });
                            FailActionCounter = -1;
                            ProcessingAction = false;
                            Logger.Info(PREFIX + "Successfully commended " + commend.SteamID.ConvertToUInt64() + ".");
                            break;
                        }
                        if(response.Body.account_idSpecified && report.SteamID.AccountID != response.Body.account_id)
                        {
                            Logger.Warning(PREFIX + "Something wrong happened,maybe we just failed a report and recieved it's response(1).");
                            break;
                        }
                        actionQueue.Dequeue();
                    }
                    MedusaWebServer.addLog(new Dictionary<string,object>()
                    {
                        { "type", "report" },
                        { "username", Username },
                        { "steamid", report.SteamID.ConvertToUInt64().ToString() },
                        { "matchid", report.MatchID.ToString() },
                        { "reportid", response.Body.confirmation_id.ToString() },
                        { "time", Utils.Time() },
                    });
                    FailActionCounter = -1;
                    ProcessingAction = false;
                    Logger.Info(PREFIX + "Successfully reported " + report.SteamID.ConvertToUInt64() + ",Confirmation ID:" + response.Body.confirmation_id);
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList:
                {
                    GetLiveGameInfo getLiveGame;
                    lock(actionQueue)
                    {
                        getLiveGame = actionQueue.Peek() as GetLiveGameInfo;
                        if(getLiveGame == null)
                        {
                            Logger.Warning(PREFIX + "Something wrong happened,maybe we just failed a GetLiveGame and recieved it's response.");
                            break;
                        }
                        actionQueue.Dequeue();
                    }
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg);
                    var matches = new List<Dictionary<string,object>>();
                    response.Body.matches.ForEach((match) =>
                    {
                        if(match.matchidSpecified)
                        {
                            var data = new Dictionary<string,object>()
                            {
                                { "id", match.matchid },
                                { "map", match.watchablematchinfo.game_map },
                                { "status", new Dictionary<string,object>() }
                            };
                            int i = 0;
                            match.roundstats_legacy.reservation.account_ids.ForEach((id) =>
                            {
                                ((Dictionary<string,object>)data["status"])[(id + 76561197960265728ul).ToString()] = new Dictionary<string,int>()
                                {
                                    { "kill", match.roundstats_legacy.kills[i] },
                                    { "assist", match.roundstats_legacy.assists[i] },
                                    { "death", match.roundstats_legacy.deaths[i] },
                                    { "score", match.roundstats_legacy.scores[i] },
                                    { "mvp", match.roundstats_legacy.mvps[i] }
                                };
                                i++;
                            });
                            matches.Add(data);
                        }
                    });
                    MedusaWebServer.addLog(new Dictionary<string,object>()
                    {
                        { "type", "matches" },
                        { "username", Username },
                        { "steamid", getLiveGame.SteamID.ConvertToUInt64().ToString() },
                        { "matches", matches },
                        { "time", Utils.Time() },
                    });
                    FailActionCounter = -1;
                    ProcessingAction = false;
                    Logger.Info(PREFIX + "Match info of " + getLiveGame.SteamID.ConvertToUInt64() + " recieved,there's " + matches.Count + " match(es).");
                }
                break;
            }
        }

        #endregion
    }
}
