using System.Linq;
using System.Collections.Generic;

using SteamKit2;
using BakaServer;
using Newtonsoft.Json;

using Medusa.utils;
using System;

namespace Medusa
{
    public class AccountManager
    {
        public static long DelayTo = 0;
        public static List<ulong> Whitelist = new List<ulong>();
        public static Queue<Account> DelayedLoginQueue = new Queue<Account>();

        public static bool IsWhitelisted(SteamID steamID)
        {
            return Whitelist.Contains(steamID.ConvertToUInt64());
        }

        public Dictionary<int,AccountCollection> AccountGroups = new Dictionary<int,AccountCollection>();

        public AccountManager(string json)
        {
            var parsed = JsonConvert.DeserializeObject<List<List<AccountJson>>>(json);
            int count = 0;
            foreach(var parsedGroup in parsed)
            {
                int index = AccountGroups.Count;
                AccountGroups.Add(index,new AccountCollection());
                foreach(var parsedAccount in parsedGroup)
                {
                    AccountGroups[index].Add(new Account(parsedAccount.Username,parsedAccount.Password,parsedAccount.Protected,parsedAccount.SharedSecret));
                    count++;
                }
            }
            Logger.Info("Successfully initialized " + count + " accounts.");
        }

        public void DelayedConnectAll()
        {
            AccountGroups.Values.ToList().ForEach((group) => group.ForEach(DelayedLoginQueue.Enqueue));
        }

        public void Tick(long Tick)
        {
            int delay = Math.Max(0,(int)(DelayTo - Utils.Time()));
            while(DelayedLoginQueue.Count != 0)
            {
                var account = DelayedLoginQueue.Dequeue();
                if(account.LoggedIn)
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
                account.AddDelayAction(delay,() => account.Connect());
            }
            DelayTo = Math.Max(DelayTo,Utils.Time()) + delay;
            foreach(var group in AccountGroups.Values)
            {
                group.TickAll(Tick);
            }
        }
    }
}
