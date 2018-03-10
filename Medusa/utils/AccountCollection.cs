using System.Linq;
using System.Collections.Generic;

namespace Medusa.utils
{
    public class AccountCollection : List<Account>
    {
        public int AvailableCount => this.Where((a) => a.Available).Count();

        public int ConnectAll()
        {
            int success = 0;
            foreach(var account in this)
            {
                account.Connect();
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
