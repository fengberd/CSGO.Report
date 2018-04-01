using System;
using System.Text;
using System.Windows.Forms;

namespace RegisterAssistance
{
    public partial class ParseDialog : Form
    {
        MainForm main;

        public ParseDialog(MainForm main)
        {
            InitializeComponent();
            this.main = main;
            if(Program.config["MailPassword",""]!="")
            {
                textBox2.Text = Program.config["MailPassword"];
                textBox2.PasswordChar = '*';
            }
        }

        private void button1_Click(object sender,EventArgs e)
        {
            string[] data;
            try
            {
                data = Encoding.UTF8.GetString(Convert.FromBase64String(textBox1.Text)).Replace("\r","").Split('\n');
            }
            catch(FormatException)
            {
                data = textBox1.Text.Replace("\r","").Split('\n');
            }
            foreach(string t in data)
            {
                var acc = t.Split(',');
                if(acc.Length >= 3)
                {
                    var split = acc[0].Split('_');
                    main.data.Add(new Account()
                    {
                        Id = int.Parse(split[split.Length - 1]),
                        Username = acc[0],
                        Password = acc[1],
                        CDK = acc[2]
                    });
                }
            }
            main.mailPassword = textBox2.Text;
            Close();
        }
    }
}
