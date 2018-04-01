using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using RegisterAssistance.captcha;

namespace RegisterAssistance
{
    public partial class CodeInputDialog : Form
    {
        public static ICaptchaProcessor captchaProcessor = null;

        public MainForm main;
        public string Result = "", CaptchaIdentifier = null;

        private Thread pull_thread = null;

        public CodeInputDialog(MainForm main,Image image,string code = "")
        {
            InitializeComponent();
            this.main = main;
            pictureBox1.Image = image;
            textBox1.Text = code;
        }

        private void CodeInputDialog_FormClosing(object sender,FormClosingEventArgs e)
        {
            if(pull_thread != null)
            {
                pull_thread.Abort();
            }
        }

        private void textBox1_KeyPress(object sender,KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                Result = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void CodeInputDialog_Load(object sender,EventArgs e)
        {
            if(captchaProcessor != null && main.checkBox_auto_process_vcode.Checked)
            {
                Top = 0;
                Left = Screen.FromControl(this).Bounds.Right - Width;
                CaptchaIdentifier = captchaProcessor.submitImage(pictureBox1.Image);
                if(CaptchaIdentifier != null)
                {
                    pull_thread = new Thread(new ThreadStart(() =>
                    {
                        while(true)
                        {
                            Thread.Sleep(500);
                            var code = captchaProcessor.getResult(CaptchaIdentifier);
                            if(code != null)
                            {
                                if(code.Length != 6)
                                {
                                    captchaProcessor.reportError(CaptchaIdentifier);
                                    CaptchaIdentifier = captchaProcessor.submitImage(pictureBox1.Image);
                                    if(CaptchaIdentifier == null)
                                    {
                                        break;
                                    }
                                    continue;
                                }
                                Invoke(new Action(() =>
                                {
                                    Result = code;
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }));
                                break;
                            }
                        }
                    }));
                    pull_thread.Start();
                }
            }
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            if(ModifierKeys == Keys.None && key == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessDialogKey(key);
        }
    }
}
