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
            this.SuspendLayout();
            // 
            // htmlToolTip1
            // 
            this.htmlToolTip1.OwnerDraw = true;
            // 
            // northTile
            // 
            this.northTile.ActiveControl = null;
            this.northTile.Location = new System.Drawing.Point(577, 364);
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
            this.southTile.Location = new System.Drawing.Point(577, 458);
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
            this.westTile.Location = new System.Drawing.Point(535, 411);
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
            this.eastTile.Location = new System.Drawing.Point(617, 411);
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
            this.outputTextBox.Location = new System.Drawing.Point(23, 63);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(485, 342);
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
            this.inputBox.Location = new System.Drawing.Point(24, 428);
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
            this.sendButton.Location = new System.Drawing.Point(24, 458);
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
            this.clientListBox.Location = new System.Drawing.Point(535, 63);
            this.clientListBox.Name = "clientListBox";
            this.clientListBox.Size = new System.Drawing.Size(158, 277);
            this.clientListBox.TabIndex = 5;
            this.clientListBox.SelectedIndexChanged += new System.EventHandler(this.clientListBox_SelectedIndexChanged);
            // 
            // Mud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 525);
            this.Controls.Add(this.clientListBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.southTile);
            this.Controls.Add(this.eastTile);
            this.Controls.Add(this.westTile);
            this.Controls.Add(this.northTile);
            this.Name = "Mud";
            this.Text = "MUD";
            this.Load += new System.EventHandler(this.Mud_Load);
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
    }
}

