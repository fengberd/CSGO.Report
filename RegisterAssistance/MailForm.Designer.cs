namespace RegisterAssistance
{
    partial class MailForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_guard_auto = new System.Windows.Forms.CheckBox();
            this.checkBox_guard_delete = new System.Windows.Forms.CheckBox();
            this.button_guard_clear = new System.Windows.Forms.Button();
            this.listView_guard = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_verify_auto = new System.Windows.Forms.CheckBox();
            this.checkBox_verify_delete = new System.Windows.Forms.CheckBox();
            this.button_verify_clear = new System.Windows.Forms.Button();
            this.listView_verify = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_guard_auto);
            this.groupBox1.Controls.Add(this.checkBox_guard_delete);
            this.groupBox1.Controls.Add(this.button_guard_clear);
            this.groupBox1.Controls.Add(this.listView_guard);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 284);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Steam Guard";
            // 
            // checkBox_guard_auto
            // 
            this.checkBox_guard_auto.AutoSize = true;
            this.checkBox_guard_auto.Location = new System.Drawing.Point(138, 259);
            this.checkBox_guard_auto.Name = "checkBox_guard_auto";
            this.checkBox_guard_auto.Size = new System.Drawing.Size(48, 16);
            this.checkBox_guard_auto.TabIndex = 17;
            this.checkBox_guard_auto.Text = "Auto";
            this.checkBox_guard_auto.UseVisualStyleBackColor = true;
            // 
            // checkBox_guard_delete
            // 
            this.checkBox_guard_delete.AutoSize = true;
            this.checkBox_guard_delete.Checked = true;
            this.checkBox_guard_delete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_guard_delete.Location = new System.Drawing.Point(6, 259);
            this.checkBox_guard_delete.Name = "checkBox_guard_delete";
            this.checkBox_guard_delete.Size = new System.Drawing.Size(126, 16);
            this.checkBox_guard_delete.TabIndex = 15;
            this.checkBox_guard_delete.Text = "Delete after copy";
            this.checkBox_guard_delete.UseVisualStyleBackColor = true;
            // 
            // button_guard_clear
            // 
            this.button_guard_clear.Location = new System.Drawing.Point(192, 255);
            this.button_guard_clear.Name = "button_guard_clear";
            this.button_guard_clear.Size = new System.Drawing.Size(55, 23);
            this.button_guard_clear.TabIndex = 14;
            this.button_guard_clear.Text = "Clear";
            this.button_guard_clear.UseVisualStyleBackColor = true;
            this.button_guard_clear.Click += new System.EventHandler(this.button_guard_clear_Click);
            // 
            // listView_guard
            // 
            this.listView_guard.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_guard.FullRowSelect = true;
            this.listView_guard.GridLines = true;
            this.listView_guard.Location = new System.Drawing.Point(6, 20);
            this.listView_guard.Name = "listView_guard";
            this.listView_guard.Size = new System.Drawing.Size(241, 229);
            this.listView_guard.TabIndex = 13;
            this.listView_guard.UseCompatibleStateImageBehavior = false;
            this.listView_guard.View = System.Windows.Forms.View.Details;
            this.listView_guard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_guard_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Username";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Code";
            this.columnHeader2.Width = 100;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_verify_auto);
            this.groupBox2.Controls.Add(this.checkBox_verify_delete);
            this.groupBox2.Controls.Add(this.button_verify_clear);
            this.groupBox2.Controls.Add(this.listView_verify);
            this.groupBox2.Location = new System.Drawing.Point(271, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 284);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Account Verification";
            // 
            // checkBox_verify_auto
            // 
            this.checkBox_verify_auto.AutoSize = true;
            this.checkBox_verify_auto.Checked = true;
            this.checkBox_verify_auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_verify_auto.Location = new System.Drawing.Point(138, 259);
            this.checkBox_verify_auto.Name = "checkBox_verify_auto";
            this.checkBox_verify_auto.Size = new System.Drawing.Size(48, 16);
            this.checkBox_verify_auto.TabIndex = 16;
            this.checkBox_verify_auto.Text = "Auto";
            this.checkBox_verify_auto.UseVisualStyleBackColor = true;
            // 
            // checkBox_verify_delete
            // 
            this.checkBox_verify_delete.AutoSize = true;
            this.checkBox_verify_delete.Checked = true;
            this.checkBox_verify_delete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_verify_delete.Location = new System.Drawing.Point(6, 259);
            this.checkBox_verify_delete.Name = "checkBox_verify_delete";
            this.checkBox_verify_delete.Size = new System.Drawing.Size(126, 16);
            this.checkBox_verify_delete.TabIndex = 15;
            this.checkBox_verify_delete.Text = "Delete after open";
            this.checkBox_verify_delete.UseVisualStyleBackColor = true;
            // 
            // button_verify_clear
            // 
            this.button_verify_clear.Location = new System.Drawing.Point(192, 255);
            this.button_verify_clear.Name = "button_verify_clear";
            this.button_verify_clear.Size = new System.Drawing.Size(55, 23);
            this.button_verify_clear.TabIndex = 14;
            this.button_verify_clear.Text = "Clear";
            this.button_verify_clear.UseVisualStyleBackColor = true;
            this.button_verify_clear.Click += new System.EventHandler(this.button_verify_clear_Click);
            // 
            // listView_verify
            // 
            this.listView_verify.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listView_verify.FullRowSelect = true;
            this.listView_verify.GridLines = true;
            this.listView_verify.Location = new System.Drawing.Point(6, 20);
            this.listView_verify.Name = "listView_verify";
            this.listView_verify.Size = new System.Drawing.Size(241, 229);
            this.listView_verify.TabIndex = 13;
            this.listView_verify.UseCompatibleStateImageBehavior = false;
            this.listView_verify.View = System.Windows.Forms.View.Details;
            this.listView_verify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_verify_MouseDoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "URL";
            this.columnHeader3.Width = 220;
            // 
            // MailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 308);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mail Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MailForm_FormClosed);
            this.Load += new System.EventHandler(this.MailForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_guard_clear;
        private System.Windows.Forms.ListView listView_guard;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox checkBox_guard_delete;
        private System.Windows.Forms.CheckBox checkBox_guard_auto;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_verify_auto;
        private System.Windows.Forms.CheckBox checkBox_verify_delete;
        private System.Windows.Forms.Button button_verify_clear;
        private System.Windows.Forms.ListView listView_verify;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}