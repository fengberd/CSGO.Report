using System.Linq;
using System.Collections.Generic;

namespace Medusa.utils
{
    public class AccountCollection : List<Account>
    {
        public int AvailableCount => this.Where((a) => a.LoggedIn).Count();

        public void TickAll(long Tick)
        {
            foreach(var account in this)
            {
                account.Tick(Tick);
            }
        }
    }
}
