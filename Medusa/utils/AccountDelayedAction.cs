using System;

namespace Medusa.utils
{
    public class AccountDelayedAction
    {
        public Action Action;
        public uint SecondsRemain = 0;
    }
}
