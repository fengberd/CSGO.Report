using System.Drawing;
using System.Windows.Forms;

namespace RegisterAssistance
{
    public partial class CodeInputDialog : Form
    {
        public MainForm main;
        public string Result = "";

        public CodeInputDialog(MainForm main,Image image,string code = "")
        {
            InitializeComponent();
            this.main = main;
            pictureBox1.Image = image;
            textBox1.Text = code;
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
    }
}
