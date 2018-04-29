using System;
using System.Linq;
using System.Collections.Generic;

using SteamKit2;
using BakaServer;
using Newtonsoft.Json;

using Medusa.utils;

namespace Medusa
{
    public class AccountManager
    {
        public static long DelayTo = 0;
        public static List<ulong> Whitelist = new List<ulong>();
        public static Queue<Account> DelayedLoginQueue = new Queue<Account>();

        public static bool IsWhitelisted(SteamID steamID) => Whitelist.Contains(steamID.ConvertToUInt64());

        public Dictionary<string,Account> Accounts = new Dictionary<string,Account>();

        public AccountManager(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ICollection<AccountJson>>(json);
            int count = 0;
            foreach(var parsedAccount in parsed)
            {
                Accounts.Add(parsedAccount.Username,new Account(parsedAccount.Username,parsedAccount.Password,parsedAccount.Protected,parsedAccount.SharedSecret));
                count++;
            }
            Logger.Info("Successfully initialized " + count + " accounts.");
        }

        public void DelayedConnectAll()
        {
            Accounts.Values.ToList().ForEach(DelayedLoginQueue.Enqueue);
        }

        public void Tick(long Tick)
        {
            int delay = Math.Max(0,(int)(DelayTo - Utils.Time()));
            while(DelayedLoginQueue.Count != 0)
            {
                var account = DelayedLoginQueue.Dequeue();
                if(account.LoggedIn || account.Disabled)
                {
                    continue;
                }
                delay += 15;
                if(account.sentry.Exists)
                {
                    delay -= 5;
                }
                if(Account.LoginKeys.ContainsKey(account.Username))
                {
                    delay -= 5;
                }
                else if(account.SharedSecret != "")
                {
                    delay -= 5;
                }
                account.AddDelayedAction(delay,() => account.Connect());
            }
            DelayTo = Math.Max(DelayTo,Utils.Time()) + delay;
            Accounts.Values.ToList().ForEach((a) => a.Tick(Tick));
            if(Tick % 10 == 0)
            {
                foreach(var account in Accounts.Values)
                {
                    if(account.ProcessSendQueue())
                    {
                        break;
                    }
                }
            }
        }
    }
}
