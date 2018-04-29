using System;

namespace Medusa.utils
{
    public class AccountDelayedAction
    {
        public Action Action;
        public int SecondsRemain = 0;
    }
}
