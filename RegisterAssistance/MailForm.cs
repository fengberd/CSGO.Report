using System;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using S22.Imap;

namespace RegisterAssistance
{
    public partial class MailForm : Form
    {
        public MainForm main;
        public ImapClient client = new ImapClient("outlook.office365.com",993,true);

        public MailForm(MainForm main)
        {
            InitializeComponent();
            this.main = main;
        }

        public string getSteamGuard(string name)
        {
            foreach(ListViewItem item in listView_guard.Items)
            {
                if(item.SubItems[0].Text.ToLower() == name.ToLower())
                {
                    return item.SubItems[1].Text;
                }
            }
            return null;
        }

        private void deleteMail(uint id)
        {
            client.DeleteMessage(id);
            client.Expunge();
        }

        private void processMessage(uint id)
        {
            var message = client.GetMessage(id,FetchOptions.TextOnly);
            var data = message.Body.Replace("\r\n","\n");
            if(data.Contains("Here is the Steam Guard code you need to login to account "))
            {
                var match = new Regex("Here is the Steam Guard code you need to login to account (.+)\\:\n\n([A-Z0-9]{5})\n").Match(data);
                if(match.Success)
                {
                    deleteMail(id);
                    listView_guard.Items.Insert(0,new ListViewItem(new string[]
                    {
                        match.Groups[1].Value.ToLower(),
                        match.Groups[2].Value
                    }));
                    if(checkBox_guard_auto.Checked)
                    {

                        main.browser.MainFrame.ExecuteJavaScriptAsync("if(jQuery('#authcode').length!=0)" +
                            "{" +
                                "window.CLoginPromptManager.prototype.OnTransferComplete=function()" +
                                "{" +
                                    "this.LoginComplete();" +
                                "};" +
                                "jQuery('#authcode').val('"+ match.Groups[2].Value + "');" +
                                "jQuery('[data-modalstate=\"submit\"]').click();" +
                            "}");
                    }
                }
            }
            else if(data.Contains("Create My Account"))
            {
                var match = new Regex("(https?\\://store\\.steampowered\\.com/account/newaccountverification.+)").Match(data);
                if(match.Success)
                {
                    deleteMail(id);
                    listView_verify.Items.Insert(0,match.Groups[1].Value);
                    if(checkBox_verify_auto.Checked)
                    {
                        main.chromeBrowser_mail.Load(match.Groups[1].Value);
                    }
                }
            }
            else if(data.Contains("Your activation of <b>Counter-Strike: Global Offensive Retail</b> was\nsuccessful"))
            {
                deleteMail(id);
            }
        }

        private void MailForm_Load(object sender,EventArgs e)
        {
            try
            {
                client.Login("report_bot@berd.moe",main.mailPassword,AuthMethod.Login);
            }
            catch { }
            if(!client.Authed)
            {
                MessageBox.Show("Can't log in to mail server.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Close();
            }
            else if(!client.Supports("IDLE"))
            {
                MessageBox.Show("Server doesn't support IDLE.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Close();
            }
            else
            {
                client.IdleError += (s,ev) =>
                {
                    Console.WriteLine(ev.Exception);
                };
                client.NewMessage += (s,ev) => processMessage(ev.MessageUID);
                new Thread(new ThreadStart(() =>
                {
                    while(!IsDisposed)
                    {
                        var mails = client.Search(SearchCondition.Undeleted().And(SearchCondition.Unseen()).And(SearchCondition.From("noreply@steampowered.com")));
                        foreach(uint id in mails)
                        {
                            processMessage(id);
                        }
                        Thread.Sleep(1000);
                    }
                }))
                {
                    IsBackground = true
                }.Start();
            }
        }

        private void MailForm_FormClosed(object sender,FormClosedEventArgs e)
        {
            main.Close();
        }

        private void button_guard_clear_Click(object sender,EventArgs e)
        {
            listView_guard.Items.Clear();
        }

        private void listView_guard_MouseDoubleClick(object sender,MouseEventArgs e)
        {
            if(listView_guard.SelectedItems.Count != 0)
            {
                Clipboard.SetText(listView_guard.SelectedItems[0].SubItems[1].Text);
                if(checkBox_guard_delete.Checked)
                {
                    listView_guard.Items.Remove(listView_guard.SelectedItems[0]);
                }
            }
        }

        private void button_verify_clear_Click(object sender,EventArgs e)
        {
            listView_verify.Items.Clear();
        }

        private void listView_verify_MouseDoubleClick(object sender,MouseEventArgs e)
        {
            if(listView_verify.SelectedItems.Count != 0)
            {
                main.browser.MainFrame.ExecuteJavaScriptAsync("window.open('" + listView_verify.SelectedItems[0].Text + "');");
                if(checkBox_verify_delete.Checked)
                {
                    listView_verify.Items.Remove(listView_verify.SelectedItems[0]);
                }
            }
        }
    }
}
