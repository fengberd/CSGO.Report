using System;

using SteamKit2;
using SteamKit2.GC;
using SteamKit2.Internal;
using SteamKit2.GC.CSGO.Internal;
using System.Collections.Generic;

using BakaServer;

namespace Medusa.utils
{
    public class Account
    {
        public const int APPID_CSGO = 730;

        public bool Protected = false, Available = false, Idle = true;
        public string Username, Password, SharedSecret;

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

            steamClient = new SteamClient(SteamConfiguration.Create(builder =>
            {
                builder.WithConnectionTimeout(TimeSpan.FromSeconds(30));
            }));
            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();
            steamGameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            callbackManager = new CallbackManager(steamClient);
            callbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            callbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            callbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
        }

        public void Tick(long Tick)
        {
            callbackManager.RunCallbacks();
            if(Idle && reportQueue.Count!=0)
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
            }
            if(Tick % 20 == 0)
            {
                var to_remove = new List<AccountDelayAction>();
                foreach(var action in actions)
                {
                    if(--action.SecondsRemain == 0)
                    {
                        action.Action.Invoke();
                        to_remove.Add(action);
                    }
                }
                to_remove.ForEach((a) => actions.Remove(a));
            }
        }

        public bool Connect()
        {
            if(steamClient.SteamID != null)
            {
                return false;
            }
            steamClient.Connect();
            return true;
        }
        
        public void AddDelayAction(int delay,Action action)
        {
            actions.Add(new AccountDelayAction()
            {
                Action = action,
                SecondsRemain = delay
            });
        }

        #region Steam Callbacks

        public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Logger.Info("[" + Username + "] Connected to steam.");
            var random = new Random();
            steamUser.LogOn(new SteamUser.LogOnDetails()
            {
                Username = Username,
                Password = Password,
                LoginID = ((uint)random.Next(1 << 30) << 2) | (uint)random.Next(1 << 2)
            });
        }

        public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            if(!callback.UserInitiated && Available)
            {
                Logger.Info("[" + Username + "] Disconnected from steam by accident,retrying in 5 seconds.");
                AddDelayAction(5,() => Connect());
            }
        }

        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            switch(callback.Result)
            {
            case EResult.OK:
                steamFriends.SetPersonaState(EPersonaState.Online);
                Logger.Info("[" + Username + "] Successfully logged in.");
                var clientGamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
                clientGamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed()
                {
                    game_id = APPID_CSGO
                });
                steamClient.Send(clientGamesPlayed);
                AddDelayAction(2,() => steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello),APPID_CSGO));
                break;
            case EResult.AccountLogonDenied:
            case EResult.AccountLoginDeniedNeedTwoFactor:
                Logger.Error("[" + Username + "] Requires steam token to log in.");
                break;
            case EResult.InvalidPassword:
                Logger.Error("[" + Username + "] Password incorrect.");
                break;
            case EResult.Timeout:
            case EResult.NoConnection:
            case EResult.TryAnotherCM:
            case EResult.ServiceUnavailable:
            case EResult.TwoFactorCodeMismatch:
                Logger.Error("[" + Username + "] Unable to connect to Steam: " + callback.ExtendedResult + ".Retrying...");
                AddDelayAction(5,() => Connect());
                break;
            case EResult.RateLimitExceeded:
                Logger.Error("[" + Username + "] Steam Rate Limit has been reached.Retrying in 1 minute...");
                AddDelayAction(60,() => Connect());
                break;
            case EResult.AccountDisabled:
                Logger.Error("[" + Username + "] has been permanently disabled by the Steam network.");
                break;
            default:
                Logger.Error("[" + Username + "] Unable to login: " + callback.Result + "(" + callback.ExtendedResult + ").");
                break;
            }
        }

        public void OnGCMessage(SteamGameCoordinator.MessageCallback callback)
        {
            var msg = callback.Message;
            switch(callback.EMsg)
            {
            case (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome:
                {
                    var response = new ClientGCMsgProtobuf<CMsgClientWelcome>(msg);
                    Logger.Debug("[" + Username + "] Connected to " + response.Body.location.country + " server.");
                    steamGameCoordinator.Send(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello),APPID_CSGO);
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello:
                {
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg);
                    if(response.Body.penalty_reasonSpecified)
                    {
                        switch(response.Body.penalty_reason)
                        {
                        case 10:
                            Logger.Error("[" + Username + "] Account has been convicted by Overwatch as majorly disruptive.");
                            return;
                        case 11:
                            Logger.Error("[" + Username + "] Account has been convicted by Overwatch as minorly disruptive.");
                            return;
                        case 14:
                            Logger.Error("[" + Username + "] Account is permanently untrusted.");
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

                                    Logger.Warning("[" + Username + "] Account has received a Matchmaking cooldown.Retrying in " + penalty.Seconds + " seconds.");
                                    steamClient.Disconnect();
                                    AddDelayAction(penalty.Seconds,() => Connect());
                                    return;
                                }
                            }
                            Logger.Error("[" + Username + "] Account has been permanently banned from CS:GO.");
                            return;
                        }
                    }
                    else if(response.Body.vac_bannedSpecified && response.Body.vac_banned == 2 && !response.Body.penalty_secondsSpecified)
                    {
                        Logger.Error("[" + Username + "] Account has been banned by VAC,VOLVO ARE YOU KIDDING ME????");
                        return;
                    }
                    Available = true;
                }
                break;
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList:
            case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientReportResponse:
                {
                    var report = reportQueue.Dequeue();
                    var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientReportResponse>(msg);
                    MedusaWebServer.addReportLog(new Dictionary<string,string>()
                    {
                        { "username",Username },
                        { "steamid", report.SteamID.ToString() },
                        { "match", report.MatchID.ToString() },
                        { "reportid", response.Body.confirmation_id.ToString() },
                        { "time", Utils.Time().ToString() },
                    });
                    Idle = true;
                    Logger.Info("[" + Username + "] Successfully reported " + report.SteamID + ",Confirmation ID:" + response.Body.confirmation_id);
                }
                break;
            }
        }

        #endregion
    }
}
