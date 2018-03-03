using System;
using System.Text;
using System.Windows.Forms;

namespace RegisterAssistance
{
    public partial class ParseForm : Form
    {
        MainForm main;

        public ParseForm(MainForm main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void button1_Click(object sender,EventArgs e)
        {
            var data = Encoding.UTF8.GetString(Convert.FromBase64String(textBox1.Text)).Replace("\r","").Split('\n');
            foreach(string t in data)
            {
                var acc = t.Split(',');
                if(acc.Length >= 3)
                {
                    main.data.Add(new Account()
                    {
                        Id = int.Parse(acc[0].Substring(12)),
                        Username = acc[0],
                        Password = acc[1],
                        CDK = acc[2]
                    });
                }
            }

            Close();
        }
    }
}
