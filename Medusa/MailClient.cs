using System.Threading;
using System.Text.RegularExpressions;

using S22.Imap;
using BakaServer;
using System;

namespace Medusa
{
    public class MailClient
    {
        public const string PREFIX = "[Mail] ";
        public ImapClient client = new ImapClient(Program.config["MailServer","outlook.office365.com"],Program.config.GetInt("MailPort",993),Program.config.GetBool("MailTLS",true));

        public MailClient()
        {
            try
            {
                client.Login(Program.config["MailUsername"],Program.config["MailPassword"],AuthMethod.Auto);
            }
            catch { }
            if(!client.Authed)
            {
                Logger.Error(PREFIX + "Can't log in to mail server.");
                Program.Pause();
            }
            else
            {
                if(client.Supports("IDLE"))
                {
                    client.IdleError += (s,ev) =>
                    {
                        Logger.Error(ev.Exception);
                    };
                    client.NewMessage += (s,ev) => processMessage(ev.MessageUID);
                }
                new Thread(new ThreadStart(() =>
                {
                    while(true)
                    {
                        try
                        {
                            var mails = client.Search(SearchCondition.Undeleted().And(SearchCondition.Unseen()).And(SearchCondition.From("noreply@steampowered.com")));
                            foreach(uint id in mails)
                            {
                                processMessage(id);
                            }
                            Thread.Sleep(500);
                        }
                        catch
                        {
                            if(!client.Authed)
                            {
                                try
                                {
                                    client.Login(Program.config["MailUsername"],Program.config["MailPassword"],AuthMethod.Auto);
                                }
                                catch { }
                            }
                        }
                    }
                }))
                {
                    IsBackground = true
                }.Start();
                Logger.Info(PREFIX + "Successfully logged in to mail server.");
            }
        }

        private void deleteMail(uint id)
        {
            try
            {
                client.DeleteMessage(id);
                client.Expunge();
            }
            catch(Exception e)
            {
                Logger.Error(PREFIX + "Unable to delete mail " + e + ":");
                Logger.Error(e);
            }
        }

        private void processMessage(uint id,bool retry = true)
        {
            try
            {
                var message = client.GetMessage(id,FetchOptions.TextOnly,false);
                var data = message.Body.Replace("\r\n","\n");
                if(data.Contains("Here is the Steam Guard code you need to login to account "))
                {
                    var match = new Regex("Here is the Steam Guard code you need to login to account (.+)\\:\n\n([A-Z0-9]{5})\n").Match(data);
                    if(match.Success)
                    {
                        if(Program.MailCodeRecieved(match.Groups[1].Value.ToLower(),match.Groups[2].Value))
                        {
                            deleteMail(id);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error(PREFIX + "Unable to recieve mail " + e + ":");
                Logger.Error(e);
                if(retry)
                {
                    Thread.Sleep(1000);
                    processMessage(id,false);
                }
            }
        }
    }
}
