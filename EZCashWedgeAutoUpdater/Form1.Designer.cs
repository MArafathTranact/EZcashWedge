namespace EZCashWedgeAutoUpdater
{
    partial class frmAutoUpdater
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAutoUpdater));
            txtServiceDownloadURL = new TextBox();
            lblVersion = new Label();
            lblUsername = new Label();
            txtUserName = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblService = new Label();
            txtServiceInstallPath = new TextBox();
            lblActiveInstallation = new Label();
            chkbxActiveInstallation = new CheckBox();
            btnSaveConfiguration = new Button();
            btnCancel = new Button();
            btnShowPassword = new Button();
            btnFolderSelect = new Button();
            lblStatus = new Label();
            folderselectdialog = new FolderBrowserDialog();
            tooltipShowPassword = new ToolTip(components);
            toolTipFolderSelect = new ToolTip(components);
            toolTipSaveConfiguration = new ToolTip(components);
            toolTipCancel = new ToolTip(components);
            SuspendLayout();
            // 
            // txtServiceDownloadURL
            // 
            txtServiceDownloadURL.Font = new Font("Verdana", 9F);
            txtServiceDownloadURL.Location = new Point(179, 66);
            txtServiceDownloadURL.Name = "txtServiceDownloadURL";
            txtServiceDownloadURL.Size = new Size(152, 22);
            txtServiceDownloadURL.TabIndex = 0;
            txtServiceDownloadURL.Visible = false;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.BackColor = Color.Transparent;
            lblVersion.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblVersion.ForeColor = SystemColors.HighlightText;
            lblVersion.Location = new Point(14, 69);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(158, 14);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Service Download URL";
            lblVersion.Visible = false;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.BackColor = Color.Transparent;
            lblUsername.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblUsername.ForeColor = SystemColors.HighlightText;
            lblUsername.Location = new Point(14, 108);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(80, 14);
            lblUsername.TabIndex = 3;
            lblUsername.Text = "User Name";
            lblUsername.Visible = false;
            // 
            // txtUserName
            // 
            txtUserName.Font = new Font("Verdana", 9F);
            txtUserName.Location = new Point(179, 105);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(152, 22);
            txtUserName.TabIndex = 2;
            txtUserName.Visible = false;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.BackColor = Color.Transparent;
            lblPassword.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblPassword.ForeColor = SystemColors.HighlightText;
            lblPassword.Location = new Point(14, 151);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(72, 14);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password";
            lblPassword.Visible = false;
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Verdana", 9F);
            txtPassword.Location = new Point(179, 148);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(152, 22);
            txtPassword.TabIndex = 4;
            txtPassword.Visible = false;
            // 
            // lblService
            // 
            lblService.AutoSize = true;
            lblService.BackColor = Color.Transparent;
            lblService.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblService.ForeColor = SystemColors.HighlightText;
            lblService.Location = new Point(14, 193);
            lblService.Name = "lblService";
            lblService.Size = new Size(137, 14);
            lblService.TabIndex = 7;
            lblService.Text = "Service Install Path";
            lblService.Visible = false;
            // 
            // txtServiceInstallPath
            // 
            txtServiceInstallPath.Font = new Font("Verdana", 9F);
            txtServiceInstallPath.Location = new Point(179, 190);
            txtServiceInstallPath.Name = "txtServiceInstallPath";
            txtServiceInstallPath.Size = new Size(152, 22);
            txtServiceInstallPath.TabIndex = 6;
            txtServiceInstallPath.Visible = false;
            // 
            // lblActiveInstallation
            // 
            lblActiveInstallation.AutoSize = true;
            lblActiveInstallation.BackColor = Color.Transparent;
            lblActiveInstallation.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblActiveInstallation.ForeColor = SystemColors.HighlightText;
            lblActiveInstallation.Location = new Point(14, 231);
            lblActiveInstallation.Name = "lblActiveInstallation";
            lblActiveInstallation.Size = new Size(131, 14);
            lblActiveInstallation.TabIndex = 8;
            lblActiveInstallation.Text = "Active  Installation";
            lblActiveInstallation.Visible = false;
            // 
            // chkbxActiveInstallation
            // 
            chkbxActiveInstallation.AutoSize = true;
            chkbxActiveInstallation.Location = new Point(179, 231);
            chkbxActiveInstallation.Name = "chkbxActiveInstallation";
            chkbxActiveInstallation.Size = new Size(15, 14);
            chkbxActiveInstallation.TabIndex = 9;
            chkbxActiveInstallation.UseVisualStyleBackColor = true;
            chkbxActiveInstallation.Visible = false;
            // 
            // btnSaveConfiguration
            // 
            btnSaveConfiguration.Cursor = Cursors.Hand;
            btnSaveConfiguration.Font = new Font("Verdana", 9F);
            btnSaveConfiguration.Location = new Point(100, 267);
            btnSaveConfiguration.Name = "btnSaveConfiguration";
            btnSaveConfiguration.Size = new Size(76, 30);
            btnSaveConfiguration.TabIndex = 10;
            btnSaveConfiguration.Text = "Save";
            btnSaveConfiguration.UseVisualStyleBackColor = true;
            btnSaveConfiguration.Visible = false;
            btnSaveConfiguration.Click += btnSaveConfiguration_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(190, 267);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 29);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Visible = false;
            btnCancel.ClientSizeChanged += btnCancel_Click;
            // 
            // btnShowPassword
            // 
            btnShowPassword.BackgroundImage = (Image)resources.GetObject("btnShowPassword.BackgroundImage");
            btnShowPassword.BackgroundImageLayout = ImageLayout.Stretch;
            btnShowPassword.Location = new Point(335, 148);
            btnShowPassword.Name = "btnShowPassword";
            btnShowPassword.Size = new Size(26, 23);
            btnShowPassword.TabIndex = 12;
            btnShowPassword.UseVisualStyleBackColor = true;
            btnShowPassword.Visible = false;
            btnShowPassword.CommandParameterChanged += btnShowPassword_Click;
            // 
            // btnFolderSelect
            // 
            btnFolderSelect.BackgroundImage = (Image)resources.GetObject("btnFolderSelect.BackgroundImage");
            btnFolderSelect.BackgroundImageLayout = ImageLayout.Stretch;
            btnFolderSelect.Location = new Point(335, 191);
            btnFolderSelect.Name = "btnFolderSelect";
            btnFolderSelect.Size = new Size(26, 23);
            btnFolderSelect.TabIndex = 13;
            btnFolderSelect.UseVisualStyleBackColor = true;
            btnFolderSelect.Visible = false;
            btnFolderSelect.Click += btnFolderSelect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Verdana", 9F, FontStyle.Bold);
            lblStatus.ForeColor = SystemColors.HighlightText;
            lblStatus.Location = new Point(30, 126);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 14);
            lblStatus.TabIndex = 14;
            // 
            // frmAutoUpdater
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(374, 309);
            Controls.Add(lblStatus);
            Controls.Add(btnFolderSelect);
            Controls.Add(btnShowPassword);
            Controls.Add(btnCancel);
            Controls.Add(btnSaveConfiguration);
            Controls.Add(chkbxActiveInstallation);
            Controls.Add(lblActiveInstallation);
            Controls.Add(lblService);
            Controls.Add(txtServiceInstallPath);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblUsername);
            Controls.Add(txtUserName);
            Controls.Add(lblVersion);
            Controls.Add(txtServiceDownloadURL);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAutoUpdater";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EzcashWedge Auto Updater";
            Load += frmAutoUpdater_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtServiceDownloadURL;
        private Label lblVersion;
        private Label lblUsername;
        private TextBox txtUserName;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblService;
        private TextBox txtServiceInstallPath;
        private Label lblActiveInstallation;
        private CheckBox chkbxActiveInstallation;
        private Button btnSaveConfiguration;
        private Button btnCancel;
        private Button btnShowPassword;
        private Button btnFolderSelect;
        private Label lblStatus;
        private FolderBrowserDialog folderselectdialog;
        private ToolTip tooltipShowPassword;
        private ToolTip toolTipFolderSelect;
        private ToolTip toolTipSaveConfiguration;
        private ToolTip toolTipCancel;
    }
}
