using SteamKit2;
using BakaServer;

namespace Medusa.utils
{
    public class DebugListener : IDebugListener
    {
        public void WriteLine(string category,string msg)
        {
            Logger.Debug("[SteamKit2] " + category + ": " + msg);
        }
    }
}
