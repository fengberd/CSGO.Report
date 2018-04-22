using System;

using SteamKit2;

namespace Medusa.utils
{
    public class SendInfo
    {
        public enum SendType
        {
            SteamClient,
            SteamGameCoordinator
        }

        public SendType Type;
        public object Packet;
    }
}
