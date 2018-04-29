using System;

namespace Medusa.utils.actions
{
    public class CommendInfo : ActionInfo
    {
        public static int FLAG_FRIENDLY = Convert.ToInt32("001",2),
           FLAG_GOOD_TEACHER = Convert.ToInt32("010",2),
          FLAG_GOOD_LEADER = Convert.ToInt32("100",2);

        public int Flags;
        public ulong MatchID = 8;
        public bool Friendly, GoodTeacher, GoodLeader;
    }
}
