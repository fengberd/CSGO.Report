using System.Collections.Generic;

using SteamKit2;
using BakaServer;
using Newtonsoft.Json;

using Medusa.utils;

namespace Medusa
{
    public class AccountManager
    {
        public static List<ulong> Whitelist = new List<ulong>();

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

        public int ConnectAll()
        {
            int success = 0;
            foreach(var group in AccountGroups.Values)
            {
                success += group.ConnectAll();
            }
            return success;
        }

        public void Tick(long Tick)
        {
            foreach(var group in AccountGroups.Values)
            {
                group.TickAll(Tick);
            }
        }
    }
}
