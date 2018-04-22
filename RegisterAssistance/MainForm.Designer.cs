namespace RegisterAssistance
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_steam = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.panel_mail = new System.Windows.Forms.Panel();
            this.textBox_url = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.panel_steam = new System.Windows.Forms.Panel();
            this.tabPage_settings = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox_override_profile_check = new System.Windows.Forms.CheckBox();
            this.checkBox_direct_go_2fa = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_get_vcode = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_process_vcode = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox_auto_go_profile = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_go_active = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_go_group = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_go_2fa = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_go_next_account = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox_auto_login = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_profile = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_cdk = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_avatar = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_group = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_disable_steam_guard = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_default_register = new System.Windows.Forms.RadioButton();
            this.radioButton_default_login = new System.Windows.Forms.RadioButton();
            this.timer_get_vcode = new System.Windows.Forms.Timer(this.components);
            this.checkBox_auto_privacy = new System.Windows.Forms.CheckBox();
            this.checkBox_direct_go_privacy = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage_steam.SuspendLayout();
            this.tabPage_settings.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 534);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Register Page";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(244, 534);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Profile Page";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(362, 534);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Active Page";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(849, 534);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(80, 23);
            this.button9.TabIndex = 9;
            this.button9.Text = "<";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(935, 534);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(80, 23);
            this.button8.TabIndex = 8;
            this.button8.Text = ">";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_steam);
            this.tabControl1.Controls.Add(this.tabPage_settings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1027, 591);
            this.tabControl1.TabIndex = 11;
            this.tabControl1.TabStop = false;
            // 
            // tabPage_steam
            // 
            this.tabPage_steam.Controls.Add(this.button6);
            this.tabPage_steam.Controls.Add(this.panel_mail);
            this.tabPage_steam.Controls.Add(this.textBox_url);
            this.tabPage_steam.Controls.Add(this.button10);
            this.tabPage_steam.Controls.Add(this.button7);
            this.tabPage_steam.Controls.Add(this.panel_steam);
            this.tabPage_steam.Controls.Add(this.button9);
            this.tabPage_steam.Controls.Add(this.button1);
            this.tabPage_steam.Controls.Add(this.button8);
            this.tabPage_steam.Controls.Add(this.button2);
            this.tabPage_steam.Controls.Add(this.button3);
            this.tabPage_steam.Location = new System.Drawing.Point(4, 22);
            this.tabPage_steam.Name = "tabPage_steam";
            this.tabPage_steam.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_steam.Size = new System.Drawing.Size(1019, 565);
            this.tabPage_steam.TabIndex = 0;
            this.tabPage_steam.Text = "Steam";
            this.tabPage_steam.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(598, 534);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(112, 23);
            this.button6.TabIndex = 14;
            this.button6.Text = "Two Factor Page";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // panel_mail
            // 
            this.panel_mail.Location = new System.Drawing.Point(833, 534);
            this.panel_mail.Name = "panel_mail";
            this.panel_mail.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panel_mail.Size = new System.Drawing.Size(10, 23);
            this.panel_mail.TabIndex = 0;
            // 
            // textBox_url
            // 
            this.textBox_url.Location = new System.Drawing.Point(8, 6);
            this.textBox_url.Name = "textBox_url";
            this.textBox_url.Size = new System.Drawing.Size(1007, 21);
            this.textBox_url.TabIndex = 13;
            this.textBox_url.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_url_KeyPress);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(480, 534);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(112, 23);
            this.button10.TabIndex = 12;
            this.button10.Text = "Group Page";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(126, 534);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(112, 23);
            this.button7.TabIndex = 11;
            this.button7.Text = "Login Page";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // panel_steam
            // 
            this.panel_steam.Location = new System.Drawing.Point(8, 33);
            this.panel_steam.Name = "panel_steam";
            this.panel_steam.Size = new System.Drawing.Size(1007, 495);
            this.panel_steam.TabIndex = 0;
            // 
            // tabPage_settings
            // 
            this.tabPage_settings.Controls.Add(this.groupBox4);
            this.tabPage_settings.Controls.Add(this.groupBox3);
            this.tabPage_settings.Controls.Add(this.groupBox2);
            this.tabPage_settings.Controls.Add(this.groupBox1);
            this.tabPage_settings.Location = new System.Drawing.Point(4, 22);
            this.tabPage_settings.Name = "tabPage_settings";
            this.tabPage_settings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_settings.Size = new System.Drawing.Size(1019, 565);
            this.tabPage_settings.TabIndex = 2;
            this.tabPage_settings.Text = "Settings";
            this.tabPage_settings.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.flowLayoutPanel3);
            this.groupBox4.Location = new System.Drawing.Point(656, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(209, 185);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Misc";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.checkBox_override_profile_check);
            this.flowLayoutPanel3.Controls.Add(this.checkBox_direct_go_2fa);
            this.flowLayoutPanel3.Controls.Add(this.checkBox_auto_get_vcode);
            this.flowLayoutPanel3.Controls.Add(this.checkBox_auto_process_vcode);
            this.flowLayoutPanel3.Controls.Add(this.button5);
            this.flowLayoutPanel3.Controls.Add(this.button4);
            this.flowLayoutPanel3.Controls.Add(this.checkBox_direct_go_privacy);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(203, 165);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // checkBox_override_profile_check
            // 
            this.checkBox_override_profile_check.AutoSize = true;
            this.checkBox_override_profile_check.Location = new System.Drawing.Point(3, 3);
            this.checkBox_override_profile_check.Name = "checkBox_override_profile_check";
            this.checkBox_override_profile_check.Size = new System.Drawing.Size(156, 16);
            this.checkBox_override_profile_check.TabIndex = 8;
            this.checkBox_override_profile_check.Text = "Override Profile Check";
            this.checkBox_override_profile_check.UseVisualStyleBackColor = true;
            // 
            // checkBox_direct_go_2fa
            // 
            this.checkBox_direct_go_2fa.AutoSize = true;
            this.checkBox_direct_go_2fa.Location = new System.Drawing.Point(3, 25);
            this.checkBox_direct_go_2fa.Name = "checkBox_direct_go_2fa";
            this.checkBox_direct_go_2fa.Size = new System.Drawing.Size(150, 16);
            this.checkBox_direct_go_2fa.TabIndex = 14;
            this.checkBox_direct_go_2fa.Text = "Direct go to 2FA Page";
            this.checkBox_direct_go_2fa.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_get_vcode
            // 
            this.checkBox_auto_get_vcode.AutoSize = true;
            this.checkBox_auto_get_vcode.Location = new System.Drawing.Point(3, 47);
            this.checkBox_auto_get_vcode.Name = "checkBox_auto_get_vcode";
            this.checkBox_auto_get_vcode.Size = new System.Drawing.Size(138, 16);
            this.checkBox_auto_get_vcode.TabIndex = 11;
            this.checkBox_auto_get_vcode.Text = "Auto Prompt Captcha";
            this.checkBox_auto_get_vcode.UseVisualStyleBackColor = true;
            this.checkBox_auto_get_vcode.CheckedChanged += new System.EventHandler(this.checkBox_auto_get_vcode_CheckedChanged);
            // 
            // checkBox_auto_process_vcode
            // 
            this.checkBox_auto_process_vcode.AutoSize = true;
            this.checkBox_auto_process_vcode.Checked = true;
            this.checkBox_auto_process_vcode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_process_vcode.Location = new System.Drawing.Point(3, 69);
            this.checkBox_auto_process_vcode.Name = "checkBox_auto_process_vcode";
            this.checkBox_auto_process_vcode.Size = new System.Drawing.Size(144, 16);
            this.checkBox_auto_process_vcode.TabIndex = 15;
            this.checkBox_auto_process_vcode.Text = "Auto Process Captcha";
            this.checkBox_auto_process_vcode.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("ËÎÌו", 9F);
            this.button5.Location = new System.Drawing.Point(3, 91);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(93, 23);
            this.button5.TabIndex = 12;
            this.button5.Text = "DevTools";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("ËÎÌו", 9F);
            this.button4.Location = new System.Drawing.Point(102, 91);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 23);
            this.button4.TabIndex = 13;
            this.button4.Text = "Fill Form";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.flowLayoutPanel2);
            this.groupBox3.Location = new System.Drawing.Point(330, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 185);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Auto Nevigate";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.checkBox_auto_go_profile);
            this.flowLayoutPanel2.Controls.Add(this.checkBox_auto_go_active);
            this.flowLayoutPanel2.Controls.Add(this.checkBox_auto_go_group);
            this.flowLayoutPanel2.Controls.Add(this.checkBox_auto_go_2fa);
            this.flowLayoutPanel2.Controls.Add(this.checkBox_auto_go_next_account);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(314, 165);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // checkBox_auto_go_profile
            // 
            this.checkBox_auto_go_profile.AutoSize = true;
            this.checkBox_auto_go_profile.Checked = true;
            this.checkBox_auto_go_profile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go_profile.Location = new System.Drawing.Point(3, 3);
            this.checkBox_auto_go_profile.Name = "checkBox_auto_go_profile";
            this.checkBox_auto_go_profile.Size = new System.Drawing.Size(210, 16);
            this.checkBox_auto_go_profile.TabIndex = 5;
            this.checkBox_auto_go_profile.Text = "Navigate to Profile After Login";
            this.checkBox_auto_go_profile.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_go_active
            // 
            this.checkBox_auto_go_active.AutoSize = true;
            this.checkBox_auto_go_active.Checked = true;
            this.checkBox_auto_go_active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go_active.Location = new System.Drawing.Point(3, 25);
            this.checkBox_auto_go_active.Name = "checkBox_auto_go_active";
            this.checkBox_auto_go_active.Size = new System.Drawing.Size(288, 16);
            this.checkBox_auto_go_active.TabIndex = 6;
            this.checkBox_auto_go_active.Text = "Navigate to Active Page After Update Profile";
            this.checkBox_auto_go_active.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_go_group
            // 
            this.checkBox_auto_go_group.AutoSize = true;
            this.checkBox_auto_go_group.Checked = true;
            this.checkBox_auto_go_group.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go_group.Location = new System.Drawing.Point(3, 47);
            this.checkBox_auto_go_group.Name = "checkBox_auto_go_group";
            this.checkBox_auto_go_group.Size = new System.Drawing.Size(234, 16);
            this.checkBox_auto_go_group.TabIndex = 7;
            this.checkBox_auto_go_group.Text = "Navigate to Group Page After Active";
            this.checkBox_auto_go_group.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_go_2fa
            // 
            this.checkBox_auto_go_2fa.AutoSize = true;
            this.checkBox_auto_go_2fa.Checked = true;
            this.checkBox_auto_go_2fa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go_2fa.Location = new System.Drawing.Point(3, 69);
            this.checkBox_auto_go_2fa.Name = "checkBox_auto_go_2fa";
            this.checkBox_auto_go_2fa.Size = new System.Drawing.Size(240, 16);
            this.checkBox_auto_go_2fa.TabIndex = 14;
            this.checkBox_auto_go_2fa.Text = "Navigate to 2Factor After Join Group";
            this.checkBox_auto_go_2fa.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_go_next_account
            // 
            this.checkBox_auto_go_next_account.AutoSize = true;
            this.checkBox_auto_go_next_account.Checked = true;
            this.checkBox_auto_go_next_account.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_go_next_account.Location = new System.Drawing.Point(3, 91);
            this.checkBox_auto_go_next_account.Name = "checkBox_auto_go_next_account";
            this.checkBox_auto_go_next_account.Size = new System.Drawing.Size(282, 16);
            this.checkBox_auto_go_next_account.TabIndex = 14;
            this.checkBox_auto_go_next_account.Text = "Goto Next Account After Disable Steam Guard";
            this.checkBox_auto_go_next_account.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 185);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Fill && Click";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_login);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_profile);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_privacy);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_cdk);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_avatar);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_group);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_auto_disable_steam_guard);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(312, 165);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // checkBox_auto_login
            // 
            this.checkBox_auto_login.AutoSize = true;
            this.checkBox_auto_login.Checked = true;
            this.checkBox_auto_login.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_login.Location = new System.Drawing.Point(3, 3);
            this.checkBox_auto_login.Name = "checkBox_auto_login";
            this.checkBox_auto_login.Size = new System.Drawing.Size(84, 16);
            this.checkBox_auto_login.TabIndex = 0;
            this.checkBox_auto_login.Text = "Auto Login";
            this.checkBox_auto_login.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_profile
            // 
            this.checkBox_auto_profile.AutoSize = true;
            this.checkBox_auto_profile.Checked = true;
            this.checkBox_auto_profile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_profile.Location = new System.Drawing.Point(3, 25);
            this.checkBox_auto_profile.Name = "checkBox_auto_profile";
            this.checkBox_auto_profile.Size = new System.Drawing.Size(264, 16);
            this.checkBox_auto_profile.TabIndex = 1;
            this.checkBox_auto_profile.Text = "Auto Save Profile(After avatar uploaded)";
            this.checkBox_auto_profile.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_cdk
            // 
            this.checkBox_auto_cdk.AutoSize = true;
            this.checkBox_auto_cdk.Checked = true;
            this.checkBox_auto_cdk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_cdk.Location = new System.Drawing.Point(3, 69);
            this.checkBox_auto_cdk.Name = "checkBox_auto_cdk";
            this.checkBox_auto_cdk.Size = new System.Drawing.Size(114, 16);
            this.checkBox_auto_cdk.TabIndex = 2;
            this.checkBox_auto_cdk.Text = "Auto Active CDK";
            this.checkBox_auto_cdk.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_avatar
            // 
            this.checkBox_auto_avatar.AutoSize = true;
            this.checkBox_auto_avatar.Checked = true;
            this.checkBox_auto_avatar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_avatar.Location = new System.Drawing.Point(3, 91);
            this.checkBox_auto_avatar.Name = "checkBox_auto_avatar";
            this.checkBox_auto_avatar.Size = new System.Drawing.Size(132, 16);
            this.checkBox_auto_avatar.TabIndex = 3;
            this.checkBox_auto_avatar.Text = "Auto Upload Avatar";
            this.checkBox_auto_avatar.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_group
            // 
            this.checkBox_auto_group.AutoSize = true;
            this.checkBox_auto_group.Checked = true;
            this.checkBox_auto_group.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_group.Location = new System.Drawing.Point(3, 113);
            this.checkBox_auto_group.Name = "checkBox_auto_group";
            this.checkBox_auto_group.Size = new System.Drawing.Size(114, 16);
            this.checkBox_auto_group.TabIndex = 4;
            this.checkBox_auto_group.Text = "Auto Join Group";
            this.checkBox_auto_group.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_disable_steam_guard
            // 
            this.checkBox_auto_disable_steam_guard.AutoSize = true;
            this.checkBox_auto_disable_steam_guard.Checked = true;
            this.checkBox_auto_disable_steam_guard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_disable_steam_guard.Location = new System.Drawing.Point(3, 135);
            this.checkBox_auto_disable_steam_guard.Name = "checkBox_auto_disable_steam_guard";
            this.checkBox_auto_disable_steam_guard.Size = new System.Drawing.Size(168, 16);
            this.checkBox_auto_disable_steam_guard.TabIndex = 15;
            this.checkBox_auto_disable_steam_guard.Text = "Auto Disable Steam Guard";
            this.checkBox_auto_disable_steam_guard.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_default_register);
            this.groupBox1.Controls.Add(this.radioButton_default_login);
            this.groupBox1.Location = new System.Drawing.Point(871, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 42);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Page";
            // 
            // radioButton_default_register
            // 
            this.radioButton_default_register.AutoSize = true;
            this.radioButton_default_register.Checked = true;
            this.radioButton_default_register.Location = new System.Drawing.Point(65, 20);
            this.radioButton_default_register.Name = "radioButton_default_register";
            this.radioButton_default_register.Size = new System.Drawing.Size(71, 16);
            this.radioButton_default_register.TabIndex = 11;
            this.radioButton_default_register.TabStop = true;
            this.radioButton_default_register.Text = "Register";
            this.radioButton_default_register.UseVisualStyleBackColor = true;
            // 
            // radioButton_default_login
            // 
            this.radioButton_default_login.AutoSize = true;
            this.radioButton_default_login.Location = new System.Drawing.Point(6, 20);
            this.radioButton_default_login.Name = "radioButton_default_login";
            this.radioButton_default_login.Size = new System.Drawing.Size(53, 16);
            this.radioButton_default_login.TabIndex = 10;
            this.radioButton_default_login.Text = "Login";
            this.radioButton_default_login.UseVisualStyleBackColor = true;
            // 
            // timer_get_vcode
            // 
            this.timer_get_vcode.Interval = 550;
            this.timer_get_vcode.Tick += new System.EventHandler(this.timer_get_vcode_Tick);
            // 
            // checkBox_auto_privacy
            // 
            this.checkBox_auto_privacy.AutoSize = true;
            this.checkBox_auto_privacy.Checked = true;
            this.checkBox_auto_privacy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_auto_privacy.Location = new System.Drawing.Point(3, 47);
            this.checkBox_auto_privacy.Name = "checkBox_auto_privacy";
            this.checkBox_auto_privacy.Size = new System.Drawing.Size(192, 16);
            this.checkBox_auto_privacy.TabIndex = 16;
            this.checkBox_auto_privacy.Text = "Auto Change Privacy Settings";
            this.checkBox_auto_privacy.UseVisualStyleBackColor = true;
            // 
            // checkBox_direct_go_privacy
            // 
            this.checkBox_direct_go_privacy.AutoSize = true;
            this.checkBox_direct_go_privacy.Location = new System.Drawing.Point(3, 120);
            this.checkBox_direct_go_privacy.Name = "checkBox_direct_go_privacy";
            this.checkBox_direct_go_privacy.Size = new System.Drawing.Size(174, 16);
            this.checkBox_direct_go_privacy.TabIndex = 16;
            this.checkBox_direct_go_privacy.Text = "Direct go to privacy page";
            this.checkBox_direct_go_privacy.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 591);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register Assistant";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_steam.ResumeLayout(false);
            this.tabPage_steam.PerformLayout();
            this.tabPage_settings.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_steam;
        private System.Windows.Forms.Panel panel_steam;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TabPage tabPage_settings;
        private System.Windows.Forms.CheckBox checkBox_auto_cdk;
        private System.Windows.Forms.CheckBox checkBox_auto_profile;
        private System.Windows.Forms.CheckBox checkBox_auto_login;
        private System.Windows.Forms.CheckBox checkBox_auto_avatar;
        private System.Windows.Forms.CheckBox checkBox_auto_group;
        private System.Windows.Forms.CheckBox checkBox_auto_go_profile;
        private System.Windows.Forms.CheckBox checkBox_auto_go_active;
        private System.Windows.Forms.CheckBox checkBox_auto_go_group;
        private System.Windows.Forms.CheckBox checkBox_override_profile_check;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_default_register;
        private System.Windows.Forms.RadioButton radioButton_default_login;
        private System.Windows.Forms.CheckBox checkBox_auto_get_vcode;
        private System.Windows.Forms.TextBox textBox_url;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox checkBox_auto_go_next_account;
        private System.Windows.Forms.Panel panel_mail;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox checkBox_auto_disable_steam_guard;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBox_auto_go_2fa;
        private System.Windows.Forms.CheckBox checkBox_direct_go_2fa;
        internal System.Windows.Forms.CheckBox checkBox_auto_process_vcode;
        private System.Windows.Forms.Timer timer_get_vcode;
        private System.Windows.Forms.CheckBox checkBox_auto_privacy;
        private System.Windows.Forms.CheckBox checkBox_direct_go_privacy;
    }
}