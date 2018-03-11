using System.Linq;
using System.Collections.Generic;

namespace Medusa.utils
{
    public class AccountCollection : List<Account>
    {
        public int AvailableCount => this.Where((a) => a.LoggedIn).Count();

        public int DelayedConnectAll(ref int delay)
        {
            int success = 0;
            foreach(var account in this)
            {
                account.AddDelayAction(delay,() => account.Connect());
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
            }
            return success;
        }

        public void TickAll(long Tick)
        {
            foreach(var account in this)
            {
                account.Tick(Tick);
            }
        }
    }
}
