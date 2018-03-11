using System.IO;

using SteamKit2;

namespace Medusa.utils
{
    public class SentryFile
    {
        public static string Storage = "sentry";

        public string FullPath;

        public bool Exists => File.Exists(FullPath);

        public byte[] Hash => CryptoHelper.SHAHash(File.ReadAllBytes(FullPath));

        public int Length => (int)new FileInfo(FullPath).Length;

        public SentryFile(string Username)
        {
            if(!Directory.Exists(Storage))
            {
                Directory.CreateDirectory(Storage);
            }
            this.FullPath = Path.GetFullPath(Path.Combine(Storage,Username.ToLower() + ".bin"));
        }

        public void Write(int offset,byte[] data,int length)
        {
            using(var stream = File.Open(FullPath,FileMode.OpenOrCreate))
            {
                stream.Seek(offset,SeekOrigin.Begin);
                stream.Write(data,0,length);
                stream.Close();
            }
        }
    }
}
