using System;

namespace Medusa.utils.actions
{
    public class ReportInfo : ActionInfo
    {
        public static int FLAG_ABUSIVE_TEXT = Convert.ToInt32("000001",2),
            FLAG_ABUSIVE_VOICE = Convert.ToInt32("000010",2),
            FLAG_GRIEFING = Convert.ToInt32("000100",2),
            FLAG_AIM_HACKING = Convert.ToInt32("001000",2),
            FLAG_WALL_HACKING = Convert.ToInt32("010000",2),
            FLAG_OTHER_HACKING = Convert.ToInt32("100000",2);

        public ulong MatchID;
        public bool AbusiveText, AbusiveVoice, Griefing, AimHacking, WallHacking, OtherHacking;
    }
}
