using System;
using System.Windows.Forms;

using BakaServer;

namespace RegisterAssistance
{
    static class Program
    {
        public static Config config = new Config("config.ini");

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
