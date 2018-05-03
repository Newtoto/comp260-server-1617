namespace MUDClient
{
    partial class Mud
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
            if (disposing && (components != null))
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
            this.htmlToolTip1 = new MetroFramework.Drawing.Html.HtmlToolTip();
            this.northTile = new MetroFramework.Controls.MetroTile();
            this.southTile = new MetroFramework.Controls.MetroTile();
            this.westTile = new MetroFramework.Controls.MetroTile();
            this.eastTile = new MetroFramework.Controls.MetroTile();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.inputBox = new MetroFramework.Controls.MetroTextBox();
            this.sendButton = new MetroFramework.Controls.MetroTile();
            this.clientListBox = new System.Windows.Forms.ListBox();
            this.errorDisplay = new System.Windows.Forms.Label();
            this.signUpButton = new MetroFramework.Controls.MetroTile();
            this.loginButton = new MetroFramework.Controls.MetroTile();
            this.passwordInput = new MetroFramework.Controls.MetroTextBox();
            this.usernameInput = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.loginPanel = new MetroFramework.Controls.MetroPanel();
            this.mudPanel = new MetroFramework.Controls.MetroPanel();
            this.loginPanel.SuspendLayout();
            this.mudPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // htmlToolTip1
            // 
            this.htmlToolTip1.OwnerDraw = true;
            // 
            // northTile
            // 
            this.northTile.ActiveControl = null;
            this.northTile.Location = new System.Drawing.Point(548, 401);
            this.northTile.Name = "northTile";
            this.northTile.Size = new System.Drawing.Size(76, 41);
            this.northTile.TabIndex = 0;
            this.northTile.Text = "North";
            this.northTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.northTile.UseSelectable = true;
            this.northTile.Click += new System.EventHandler(this.northTile_Click);
            // 
            // southTile
            // 
            this.southTile.ActiveControl = null;
            this.southTile.Location = new System.Drawing.Point(548, 495);
            this.southTile.Name = "southTile";
            this.southTile.Size = new System.Drawing.Size(76, 41);
            this.southTile.TabIndex = 0;
            this.southTile.Text = "South";
            this.southTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.southTile.UseSelectable = true;
            this.southTile.Click += new System.EventHandler(this.southTile_Click);
            // 
            // westTile
            // 
            this.westTile.ActiveControl = null;
            this.westTile.Location = new System.Drawing.Point(506, 448);
            this.westTile.Name = "westTile";
            this.westTile.Size = new System.Drawing.Size(76, 41);
            this.westTile.TabIndex = 0;
            this.westTile.Text = "West";
            this.westTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.westTile.UseSelectable = true;
            this.westTile.Click += new System.EventHandler(this.westTile_Click);
            // 
            // eastTile
            // 
            this.eastTile.ActiveControl = null;
            this.eastTile.Location = new System.Drawing.Point(588, 448);
            this.eastTile.Name = "eastTile";
            this.eastTile.Size = new System.Drawing.Size(76, 41);
            this.eastTile.TabIndex = 0;
            this.eastTile.Text = "East";
            this.eastTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.eastTile.UseSelectable = true;
            this.eastTile.Click += new System.EventHandler(this.eastTile_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(13, 32);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(485, 428);
            this.outputTextBox.TabIndex = 2;
            // 
            // inputBox
            // 
            // 
            // 
            // 
            this.inputBox.CustomButton.Image = null;
            this.inputBox.CustomButton.Location = new System.Drawing.Point(462, 1);
            this.inputBox.CustomButton.Name = "";
            this.inputBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inputBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inputBox.CustomButton.TabIndex = 1;
            this.inputBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inputBox.CustomButton.UseSelectable = true;
            this.inputBox.CustomButton.Visible = false;
            this.inputBox.Lines = new string[0];
            this.inputBox.Location = new System.Drawing.Point(13, 466);
            this.inputBox.MaxLength = 32767;
            this.inputBox.Name = "inputBox";
            this.inputBox.PasswordChar = '\0';
            this.inputBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inputBox.SelectedText = "";
            this.inputBox.SelectionLength = 0;
            this.inputBox.SelectionStart = 0;
            this.inputBox.ShortcutsEnabled = true;
            this.inputBox.Size = new System.Drawing.Size(484, 23);
            this.inputBox.TabIndex = 3;
            this.inputBox.UseSelectable = true;
            this.inputBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inputBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // sendButton
            // 
            this.sendButton.ActiveControl = null;
            this.sendButton.Location = new System.Drawing.Point(14, 493);
            this.sendButton.Margin = new System.Windows.Forms.Padding(0);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(484, 47);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Send";
            this.sendButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.sendButton.UseSelectable = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // clientListBox
            // 
            this.clientListBox.FormattingEnabled = true;
            this.clientListBox.Location = new System.Drawing.Point(506, 32);
            this.clientListBox.Name = "clientListBox";
            this.clientListBox.Size = new System.Drawing.Size(158, 355);
            this.clientListBox.TabIndex = 5;
            this.clientListBox.SelectedIndexChanged += new System.EventHandler(this.clientListBox_SelectedIndexChanged);
            // 
            // errorDisplay
            // 
            this.errorDisplay.AutoSize = true;
            this.errorDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.errorDisplay.Location = new System.Drawing.Point(115, 215);
            this.errorDisplay.Name = "errorDisplay";
            this.errorDisplay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.errorDisplay.Size = new System.Drawing.Size(0, 13);
            this.errorDisplay.TabIndex = 12;
            this.errorDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // signUpButton
            // 
            this.signUpButton.ActiveControl = null;
            this.signUpButton.Location = new System.Drawing.Point(200, 91);
            this.signUpButton.Name = "signUpButton";
            this.signUpButton.Size = new System.Drawing.Size(104, 45);
            this.signUpButton.TabIndex = 10;
            this.signUpButton.Text = "Sign Up";
            this.signUpButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.signUpButton.UseSelectable = true;
            this.signUpButton.Click += new System.EventHandler(this.signUpButton_Click);
            // 
            // loginButton
            // 
            this.loginButton.ActiveControl = null;
            this.loginButton.Location = new System.Drawing.Point(90, 91);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(104, 45);
            this.loginButton.TabIndex = 11;
            this.loginButton.Text = "Login";
            this.loginButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loginButton.UseSelectable = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // passwordInput
            // 
            // 
            // 
            // 
            this.passwordInput.CustomButton.Image = null;
            this.passwordInput.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.passwordInput.CustomButton.Name = "";
            this.passwordInput.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.passwordInput.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.passwordInput.CustomButton.TabIndex = 1;
            this.passwordInput.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.passwordInput.CustomButton.UseSelectable = true;
            this.passwordInput.CustomButton.Visible = false;
            this.passwordInput.Lines = new string[0];
            this.passwordInput.Location = new System.Drawing.Point(90, 53);
            this.passwordInput.MaxLength = 32767;
            this.passwordInput.Name = "passwordInput";
            this.passwordInput.PasswordChar = '\0';
            this.passwordInput.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.passwordInput.SelectedText = "";
            this.passwordInput.SelectionLength = 0;
            this.passwordInput.SelectionStart = 0;
            this.passwordInput.ShortcutsEnabled = true;
            this.passwordInput.Size = new System.Drawing.Size(214, 23);
            this.passwordInput.TabIndex = 8;
            this.passwordInput.UseSelectable = true;
            this.passwordInput.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.passwordInput.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // usernameInput
            // 
            // 
            // 
            // 
            this.usernameInput.CustomButton.Image = null;
            this.usernameInput.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.usernameInput.CustomButton.Name = "";
            this.usernameInput.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.usernameInput.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.usernameInput.CustomButton.TabIndex = 1;
            this.usernameInput.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.usernameInput.CustomButton.UseSelectable = true;
            this.usernameInput.CustomButton.Visible = false;
            this.usernameInput.Lines = new string[0];
            this.usernameInput.Location = new System.Drawing.Point(90, 18);
            this.usernameInput.MaxLength = 32767;
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.PasswordChar = '\0';
            this.usernameInput.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.usernameInput.SelectedText = "";
            this.usernameInput.SelectionLength = 0;
            this.usernameInput.SelectionStart = 0;
            this.usernameInput.ShortcutsEnabled = true;
            this.usernameInput.Size = new System.Drawing.Size(214, 23);
            this.usernameInput.TabIndex = 9;
            this.usernameInput.UseSelectable = true;
            this.usernameInput.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.usernameInput.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(21, 53);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(63, 19);
            this.metroLabel2.TabIndex = 6;
            this.metroLabel2.Text = "Password";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(16, 18);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(68, 19);
            this.metroLabel1.TabIndex = 7;
            this.metroLabel1.Text = "Username";
            // 
            // loginPanel
            // 
            this.loginPanel.Controls.Add(this.signUpButton);
            this.loginPanel.Controls.Add(this.metroLabel1);
            this.loginPanel.Controls.Add(this.loginButton);
            this.loginPanel.Controls.Add(this.metroLabel2);
            this.loginPanel.Controls.Add(this.passwordInput);
            this.loginPanel.Controls.Add(this.usernameInput);
            this.loginPanel.HorizontalScrollbarBarColor = true;
            this.loginPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.loginPanel.HorizontalScrollbarSize = 10;
            this.loginPanel.Location = new System.Drawing.Point(1, 58);
            this.loginPanel.Name = "loginPanel";
            this.loginPanel.Size = new System.Drawing.Size(333, 154);
            this.loginPanel.TabIndex = 13;
            this.loginPanel.VerticalScrollbarBarColor = true;
            this.loginPanel.VerticalScrollbarHighlightOnWheel = false;
            this.loginPanel.VerticalScrollbarSize = 10;
            // 
            // mudPanel
            // 
            this.mudPanel.Controls.Add(this.sendButton);
            this.mudPanel.Controls.Add(this.clientListBox);
            this.mudPanel.Controls.Add(this.northTile);
            this.mudPanel.Controls.Add(this.outputTextBox);
            this.mudPanel.Controls.Add(this.inputBox);
            this.mudPanel.Controls.Add(this.westTile);
            this.mudPanel.Controls.Add(this.eastTile);
            this.mudPanel.Controls.Add(this.southTile);
            this.mudPanel.HorizontalScrollbarBarColor = true;
            this.mudPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.mudPanel.HorizontalScrollbarSize = 10;
            this.mudPanel.Location = new System.Drawing.Point(23, 63);
            this.mudPanel.Name = "mudPanel";
            this.mudPanel.Size = new System.Drawing.Size(670, 552);
            this.mudPanel.TabIndex = 14;
            this.mudPanel.VerticalScrollbarBarColor = true;
            this.mudPanel.VerticalScrollbarHighlightOnWheel = false;
            this.mudPanel.VerticalScrollbarSize = 10;
            // 
            // Mud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 633);
            this.Controls.Add(this.loginPanel);
            this.Controls.Add(this.mudPanel);
            this.Controls.Add(this.errorDisplay);
            this.Name = "Mud";
            this.Text = "MUD";
            this.Load += new System.EventHandler(this.Mud_Load);
            this.loginPanel.ResumeLayout(false);
            this.loginPanel.PerformLayout();
            this.mudPanel.ResumeLayout(false);
            this.mudPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Drawing.Html.HtmlToolTip htmlToolTip1;
        private MetroFramework.Controls.MetroTile northTile;
        private MetroFramework.Controls.MetroTile southTile;
        private MetroFramework.Controls.MetroTile westTile;
        private MetroFramework.Controls.MetroTile eastTile;
        private System.Windows.Forms.TextBox outputTextBox;
        private MetroFramework.Controls.MetroTextBox inputBox;
        private MetroFramework.Controls.MetroTile sendButton;
        private System.Windows.Forms.ListBox clientListBox;
        private System.Windows.Forms.Label errorDisplay;
        private MetroFramework.Controls.MetroTile signUpButton;
        private MetroFramework.Controls.MetroTile loginButton;
        private MetroFramework.Controls.MetroTextBox passwordInput;
        private MetroFramework.Controls.MetroTextBox usernameInput;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel loginPanel;
        private MetroFramework.Controls.MetroPanel mudPanel;
    }
}

