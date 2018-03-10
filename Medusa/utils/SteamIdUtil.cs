using SteamKit2;

namespace Medusa.utils
{
    // Reference: https://github.com/Marc3842h/Titan/blob/master/Titan/Util/SteamUtil.cs
    public static class SteamIdUtil
    {
        public static SteamID Parse(string s)
        {
            switch(s[0])
            {
            case '[':
                return FromSteamID3(s);
            case 'S':
                return FromSteamID(s);
            default:
                return ulong.TryParse(s,out var id) ? FromSteamID64(id) : FromNativeUrl(s);
            }
        }

        // Renders from a "STEAM_0:0:131983088" form.
        public static SteamID FromSteamID(string steamID)
        {
            return new SteamID(steamID);
        }

        // Renders from a "[U:1:263966176]" form.
        public static SteamID FromSteamID3(string steamID3)
        {
            var id = new SteamID();
            id.SetFromSteam3String(steamID3);
            return id;
        }

        // Renders from a "76561198224231904" form.
        public static SteamID FromSteamID64(ulong steamID64)
        {
            var id = new SteamID();
            id.SetFromUInt64(steamID64);
            return id;
        }

        // Renders from a "http://steamcommunity.com/profiles/76561198224231904" form.
        public static SteamID FromNativeUrl(string nativeUrl)
        {
            if(!ulong.TryParse(nativeUrl.Replace("http://","").Replace("https://","").Replace("steamcommunity.com","").Replace("/profiles/","").Replace("/",""),out var steamID))
            {
                return null;
            }
            return FromSteamID64(steamID);
        }
    }
}
