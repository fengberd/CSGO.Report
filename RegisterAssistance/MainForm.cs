using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using CefSharp;
using CefSharp.WinForms;

namespace RegisterAssistance
{
    public partial class MainForm : Form
    {
        public int index = 0;
        public bool vcode_shown = false;
        public string mailPassword = "";

        public IBrowser browser;
        public ChromiumWebBrowser chromeBrowser_steam;

        public List<Account> data = new List<Account>();
        public List<string> avatars = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            FormClosing += (sender,e) => Cef.Shutdown();
            CheckForIllegalCrossThreadCalls = false;
            var settings = new CefSettings();
            Cef.Initialize(settings);
            panel_steam.Controls.Add(chromeBrowser_steam = new ChromiumWebBrowser("")
            {
                Dock = DockStyle.Fill
            });
            chromeBrowser_steam.FrameLoadEnd += (sender,e) =>
            {
                textBox_url.Text = e.Url;
                fillForm();
            };
            chromeBrowser_steam.IsBrowserInitializedChanged += (sender,e) =>
            {
                if(e.IsBrowserInitialized)
                {
                    browser = chromeBrowser_steam.GetBrowser();
                    timer_get_vcode.Enabled = true;
                }
            };
        }

        private string getAvatar(int id)
        {
            return avatars[id % avatars.Count];
        }

        private void loadUrl(string url)
        {
            if(url == browser.MainFrame.Url)
            {
                chromeBrowser_steam.Reload();
            }
            else
            {
                chromeBrowser_steam.Load(url);
            }
        }

        private void fillForm()
        {
            var data = this.data[index];
            var url = browser.MainFrame.Url.ToLower().Replace("http://","").Replace("https://","").Replace("//","/").TrimEnd(new char[] { '/' });
            Console.WriteLine(url);
            switch(url)
            {
            case "store.steampowered.com/join":
                browser.MainFrame.ExecuteJavaScriptAsync("jQuery('#accountname').val('" + data.Username + "');" +
                    "CheckAccountNameAvailability();" +
                    "jQuery('#password,#reenter_password').val('" + data.Password + "');" +
                    "CheckPasswordStrength();" +
                    "ReenterPasswordChange();" +
                    "jQuery('#email,#reenter_email').val('report_bot@berd.moe');" +
                    "jQuery('#i_agree_check').click();" +
                    "jQuery('.ssa_box').height(10);" +
                    "jQuery('#captcha_text').focus();" +
                    "window.scrollY=400;");
                vcode_shown = false;
                break;
            case "store.steampowered.com/account/registerkey":
                if(!checkBox_auto_cdk.Checked)
                {
                    return;
                }
                browser.MainFrame.ExecuteJavaScriptAsync("jQuery('#product_key').val('" + data.CDK + "');" +
                    "jQuery('#accept_ssa').click();" +
                    "var _lol_hacked=DisplayPage;" +
                    "DisplayPage=function(page)" +
                    "{" +
                        "_lol_hacked(page);" +
                        "if(page=='receipt')" +
                        "{" +
                            (checkBox_auto_go_group.Checked ? "document.location='http://steamcommunity.com/groups/csgo_report/';" : "") +
                        "}" +
                    "};" +
                    "RegisterProductKey();");
                break;
            case "steamcommunity.com/groups/csgo_report":
                if(!checkBox_auto_group.Checked)
                {
                    return;
                }
                browser.MainFrame.ExecuteJavaScriptAsync("if(jQuery('.grouppage_join_area .btn_green_white_innerfade').length==1)" +
                    "{" +
                        "document.forms['join_group_form'].submit();" +
                    "}");
                if(checkBox_auto_go_next_account.Checked && button8.Enabled)
                {
                    browser.MainFrame.EvaluateScriptAsync("jQuery('.grouppage_join_area .btn_blue_white_innerfade').length==1?'true':'false'").ContinueWith((r) =>
                    {
                        if(!r.IsFaulted && r.Result.Success && r.Result.Result.ToString() == "true")
                        {
                            button8.PerformClick();
                        }
                    });
                }
                break;
            case "steamcommunity.com/?go_profile":
            case "store.steampowered.com/?created_account=1":
                if(!checkBox_auto_go_profile.Checked)
                {
                    return;
                }
                browser.MainFrame.ExecuteJavaScriptAsync("var shortcut=jQuery('#account_dropdown .popup_menu_item').last().attr('href');" +
                    "document.location=shortcut.indexOf('/id/')!=-1?shortcut+'/edit':shortcut+'/edit?welcomed=1';");
                break;
            default:
                if(url.StartsWith("store.steampowered.com/login/"))
                {
                    if(!checkBox_auto_login.Checked)
                    {
                        return;
                    }
                    browser.MainFrame.ExecuteJavaScriptAsync("jQuery('#input_username').val('" + data.Username + "');" +
                        "jQuery('#input_password').val('" + data.Password + "');" +
                        "jQuery('button[type=submit]').click();");
                }
                else if(url.StartsWith("steamcommunity.com/login/"))
                {
                    if(!checkBox_auto_login.Checked)
                    {
                        return;
                    }
                    browser.MainFrame.ExecuteJavaScriptAsync("jQuery('#steamAccountName').val('" + data.Username + "');" +
                        "jQuery('#steamPassword').val('" + data.Password + "');" +
                        "jQuery('#SteamLogin').click();");
                }
                else if((url.StartsWith("steamcommunity.com/profiles/") || url.StartsWith("steamcommunity.com/id/")) && (url.EndsWith("/edit?welcomed=1") || url.EndsWith("/edit")))
                {
                    if(!checkBox_auto_avatar.Checked)
                    {
                        return;
                    }
                    browser.MainFrame.ExecuteJavaScriptAsync("if(" + (checkBox_override_profile_check.Checked ? "false)" : "jQuery('#personaName').val()=='CSGO.Report_Bot" + data.Id + "')") +
                        "{" +
                            (checkBox_auto_go_active.Checked ? "document.location='https://store.steampowered.com/account/registerkey/';" : "") +
                        "}" +
                        "else" +
                        "{" +
                            "var byteCharacters=atob('" + getAvatar(data.Id) + "');" +
                            "var byteNumbers=new Array(byteCharacters.length);" +
                            "for(var i=0;i<byteCharacters.length;i++)" +
                            "{" +
                                "byteNumbers[i]=byteCharacters.charCodeAt(i);" +
                            "}" +
                            "var data=new FormData(document.getElementById('avatar_upload_form'));" +
                            "data.append('avatar',new Blob([new Uint8Array(byteNumbers)],{type:'image/png'}));" +
                            "jQuery.ajax(" +
                            "{" +
                                "url:'http://steamcommunity.com/actions/FileUploader'," +
                                "data:data," +
                                "type:'POST'," +
                                "contentType:false," +
                                "processData:false," +
                                "cache:false," +
                                "success:function()" +
                                "{" + (checkBox_auto_profile.Checked ?
                                    "jQuery('#personaName').val('CSGO.Report_Bot" + data.Id + "');" +
                                    "jQuery('#customURL').val('csgo_report_" + data.Id + "');" +
                                    "jQuery('button[type=submit]').click();" : "") +
                                "}," +
                                "error:function(e)" +
                                "{" +
                                    "alert(e);" +
                                "}" +
                            "});" +
                        "}");
                }
                break;
            }
        }

        private void loadAccount()
        {
            button8.Enabled = index < data.Count - 1;
            button9.Enabled = index != 0;
            Text = "Register Assistant - " + data[index].Username;
            if(radioButton_default_login.Checked)
            {
                button7.PerformClick();
            }
            else
            {
                button1.PerformClick();
            }
            Cef.GetGlobalCookieManager().DeleteCookies("http://steamcommunity.com");
            Cef.GetGlobalCookieManager().DeleteCookies("https://store.steampowered.com");
        }

        private void RegisterForm_Load(object sender,EventArgs e)
        {
            if(Directory.Exists("./avatars/"))
            {
                var files = Directory.EnumerateFiles("./avatars/","*.png");
                foreach(var file in files)
                {
                    avatars.Add(Convert.ToBase64String(File.ReadAllBytes(file)));
                }
            }
            if(avatars.Count == 0)
            {
                Close();
            }
            else
            {
                new ParseDialog(this).ShowDialog();
                if(data.Count == 0)
                {
                    Close();
                }
                else
                {
                    loadAccount();
                    if(mailPassword != "")
                    {
                        new MailForm(this).Show();
                    }
                }
            }
        }

        private void button1_Click(object sender,EventArgs e)
        {
            loadUrl("https://store.steampowered.com/join/");
        }

        private void button2_Click(object sender,EventArgs e)
        {
            loadUrl("http://steamcommunity.com/?go_profile");
        }

        private void button3_Click(object sender,EventArgs e)
        {
            loadUrl("https://store.steampowered.com/account/registerkey/");
        }

        private void button4_Click(object sender,EventArgs e)
        {
            fillForm();
        }

        private void button5_Click(object sender,EventArgs e)
        {
            chromeBrowser_steam.ShowDevTools();
        }

        private void button7_Click(object sender,EventArgs e)
        {
            loadUrl("https://store.steampowered.com/login/");
        }

        private void button8_Click(object sender,EventArgs e)
        {
            index++;
            loadAccount();
        }

        private void button9_Click(object sender,EventArgs e)
        {
            index--;
            loadAccount();
        }

        private void button10_Click(object sender,EventArgs e)
        {
            loadUrl("http://steamcommunity.com/groups/csgo_report/");
        }

        private void checkBox_auto_get_vcode_CheckedChanged(object sender,EventArgs e)
        {
            timer_get_vcode.Enabled = checkBox_auto_get_vcode.Checked;
        }

        private void textBox_url_KeyPress(object sender,KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                loadUrl(textBox_url.Text);
            }
        }

        private void timer_get_vcode_Tick(object sender,EventArgs e)
        {
            if(!vcode_shown && !browser.IsLoading && browser.MainFrame.Url.ToLower().Replace("http://","").Replace("https://","").Replace("//","/") == "store.steampowered.com/join/")
            {
                browser.MainFrame.EvaluateScriptAsync("if(jQuery('#lmao_canvas').length==0)" +
                    "{" +
                        "jQuery('body').append('<canvas id=lmao_canvas />');" +
                    "}" +
                    "var c=jQuery('#lmao_canvas')[0];" +
                    "c.getContext('2d').drawImage(jQuery('#captchaImg')[0],0,0);" +
                    "c.toDataURL();").ContinueWith((r) =>
                    {
                        if(r.IsFaulted || !r.Result.Success || vcode_shown)
                        {
                            return;
                        }
                        try
                        {
                            var image = Image.FromStream(new MemoryStream(Convert.FromBase64String(r.Result.Result.ToString().Split(',')[1])));
                            vcode_shown = true;
                            var dialog = new CodeInputDialog(this,image);
                            if(dialog.ShowDialog() == DialogResult.OK)
                            {
                                browser.MainFrame.ExecuteJavaScriptAsync("jQuery('#captcha_text').val('" + dialog.Result + "');" +
                                     "CreateAccount();");
                            }
                        }
                        catch { }
                    });
            }
        }
    }
}
