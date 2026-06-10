namespace EZCashWedgeConfigurator
{
    partial class EZcashWedgeConfigurator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EZcashWedgeConfigurator));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            groupBox1 = new GroupBox();
            label6 = new Label();
            cbWedgeType = new ComboBox();
            txtArchiveRollOutDays = new TextBox();
            label5 = new Label();
            txtTraceSize = new TextBox();
            label4 = new Label();
            btnConnectEZCashAPI = new Button();
            txtWedgeIp = new TextBox();
            label1 = new Label();
            txtEZCashToken = new TextBox();
            label3 = new Label();
            txtEZCashAPI = new TextBox();
            label2 = new Label();
            btnSave = new Button();
            btnCancel = new Button();
            toolTipSaveConfiguration = new ToolTip(components);
            toolTipCancel = new ToolTip(components);
            toolTipTestAPI = new ToolTip(components);
            button1 = new Button();
            toolTipOpenConfigFile = new ToolTip(components);
            tbwedgeType = new TabControl();
            tabPage1 = new TabPage();
            gbyard = new GroupBox();
            dgYards = new DataGridView();
            tabPage2 = new TabPage();
            gbDevice = new GroupBox();
            dgDevices = new DataGridView();
            groupBox1.SuspendLayout();
            tbwedgeType.SuspendLayout();
            tabPage1.SuspendLayout();
            gbyard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgYards).BeginInit();
            tabPage2.SuspendLayout();
            gbDevice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgDevices).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Transparent;
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(cbWedgeType);
            groupBox1.Controls.Add(txtArchiveRollOutDays);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(txtTraceSize);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(btnConnectEZCashAPI);
            groupBox1.Controls.Add(txtWedgeIp);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtEZCashToken);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtEZCashAPI);
            groupBox1.Controls.Add(label2);
            groupBox1.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = SystemColors.HighlightText;
            groupBox1.Location = new Point(13, 14);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(422, 253);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "EZcash";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(15, 154);
            label6.Name = "label6";
            label6.Size = new Size(89, 14);
            label6.TabIndex = 30;
            label6.Text = "Wedge Type";
            // 
            // cbWedgeType
            // 
            cbWedgeType.Enabled = false;
            cbWedgeType.Font = new Font("Verdana", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbWedgeType.FormattingEnabled = true;
            cbWedgeType.Location = new Point(205, 151);
            cbWedgeType.Name = "cbWedgeType";
            cbWedgeType.Size = new Size(160, 22);
            cbWedgeType.TabIndex = 29;
            cbWedgeType.SelectedIndexChanged += cbWedgeType_SelectedIndexChanged;
            // 
            // txtArchiveRollOutDays
            // 
            txtArchiveRollOutDays.Enabled = false;
            txtArchiveRollOutDays.Font = new Font("Verdana", 8.25F);
            txtArchiveRollOutDays.ForeColor = SystemColors.ActiveCaptionText;
            txtArchiveRollOutDays.Location = new Point(205, 220);
            txtArchiveRollOutDays.Name = "txtArchiveRollOutDays";
            txtArchiveRollOutDays.Size = new Size(160, 21);
            txtArchiveRollOutDays.TabIndex = 28;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(15, 223);
            label5.Name = "label5";
            label5.Size = new Size(151, 14);
            label5.TabIndex = 27;
            label5.Text = "Archive Roll Out Days";
            // 
            // txtTraceSize
            // 
            txtTraceSize.Enabled = false;
            txtTraceSize.Font = new Font("Verdana", 8.25F);
            txtTraceSize.ForeColor = SystemColors.ActiveCaptionText;
            txtTraceSize.Location = new Point(205, 187);
            txtTraceSize.Name = "txtTraceSize";
            txtTraceSize.Size = new Size(160, 21);
            txtTraceSize.TabIndex = 26;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(15, 190);
            label4.Name = "label4";
            label4.Size = new Size(35, 14);
            label4.TabIndex = 25;
            label4.Text = "Size";
            // 
            // btnConnectEZCashAPI
            // 
            btnConnectEZCashAPI.Enabled = false;
            btnConnectEZCashAPI.Font = new Font("Verdana", 8F);
            btnConnectEZCashAPI.ForeColor = SystemColors.Desktop;
            btnConnectEZCashAPI.Location = new Point(302, 119);
            btnConnectEZCashAPI.Name = "btnConnectEZCashAPI";
            btnConnectEZCashAPI.Size = new Size(65, 21);
            btnConnectEZCashAPI.TabIndex = 24;
            btnConnectEZCashAPI.Text = "Connect";
            btnConnectEZCashAPI.UseVisualStyleBackColor = true;
            btnConnectEZCashAPI.Click += btnConnectEZCashAPI_Click;
            // 
            // txtWedgeIp
            // 
            txtWedgeIp.Enabled = false;
            txtWedgeIp.Font = new Font("Verdana", 8.25F);
            txtWedgeIp.ForeColor = SystemColors.ActiveCaptionText;
            txtWedgeIp.Location = new Point(205, 23);
            txtWedgeIp.Name = "txtWedgeIp";
            txtWedgeIp.Size = new Size(160, 21);
            txtWedgeIp.TabIndex = 17;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 26);
            label1.Name = "label1";
            label1.Size = new Size(80, 14);
            label1.TabIndex = 16;
            label1.Text = "Wedge Ip*";
            // 
            // txtEZCashToken
            // 
            txtEZCashToken.Enabled = false;
            txtEZCashToken.Font = new Font("Verdana", 8.25F);
            txtEZCashToken.ForeColor = SystemColors.ActiveCaptionText;
            txtEZCashToken.Location = new Point(131, 92);
            txtEZCashToken.Name = "txtEZCashToken";
            txtEZCashToken.Size = new Size(234, 21);
            txtEZCashToken.TabIndex = 15;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 95);
            label3.Name = "label3";
            label3.Size = new Size(56, 14);
            label3.TabIndex = 14;
            label3.Text = "Token*";
            // 
            // txtEZCashAPI
            // 
            txtEZCashAPI.Enabled = false;
            txtEZCashAPI.Font = new Font("Verdana", 8.25F);
            txtEZCashAPI.ForeColor = SystemColors.ActiveCaptionText;
            txtEZCashAPI.Location = new Point(131, 56);
            txtEZCashAPI.Name = "txtEZCashAPI";
            txtEZCashAPI.Size = new Size(234, 21);
            txtEZCashAPI.TabIndex = 13;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 59);
            label2.Name = "label2";
            label2.Size = new Size(40, 14);
            label2.TabIndex = 12;
            label2.Text = "API*";
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.SeaGreen;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Enabled = false;
            btnSave.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = SystemColors.Desktop;
            btnSave.Location = new Point(109, 457);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 30);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            toolTipCancel.SetToolTip(btnSave, "Close Application");
            toolTipSaveConfiguration.SetToolTip(btnSave, "Save configuration");
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Red;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.Location = new Point(208, 457);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 30);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            toolTipCancel.SetToolTip(btnCancel, "Close Application");
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.Cursor = Cursors.Hand;
            button1.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(309, 457);
            button1.Name = "button1";
            button1.Size = new Size(75, 30);
            button1.TabIndex = 4;
            toolTipOpenConfigFile.SetToolTip(button1, "Load EZCashWedge.exe.config file");
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // tbwedgeType
            // 
            tbwedgeType.Controls.Add(tabPage1);
            tbwedgeType.Controls.Add(tabPage2);
            tbwedgeType.ItemSize = new Size(1, 1);
            tbwedgeType.Location = new Point(13, 270);
            tbwedgeType.Name = "tbwedgeType";
            tbwedgeType.SelectedIndex = 0;
            tbwedgeType.Size = new Size(485, 176);
            tbwedgeType.SizeMode = TabSizeMode.Fixed;
            tbwedgeType.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(gbyard);
            tabPage1.Location = new Point(4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(477, 167);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Yard";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // gbyard
            // 
            gbyard.BackColor = Color.Transparent;
            gbyard.Controls.Add(dgYards);
            gbyard.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbyard.ForeColor = SystemColors.HighlightText;
            gbyard.Location = new Point(-4, 1);
            gbyard.Name = "gbyard";
            gbyard.Size = new Size(485, 154);
            gbyard.TabIndex = 2;
            gbyard.TabStop = false;
            gbyard.Text = "Yard Infomation*";
            gbyard.Visible = false;
            // 
            // dgYards
            // 
            dgYards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgYards.Location = new Point(11, 13);
            dgYards.Name = "dgYards";
            dataGridViewCellStyle1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dgYards.RowsDefaultCellStyle = dataGridViewCellStyle1;
            dgYards.Size = new Size(464, 147);
            dgYards.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(gbDevice);
            tabPage2.Location = new Point(4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(477, 167);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Device";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // gbDevice
            // 
            gbDevice.BackColor = Color.Transparent;
            gbDevice.Controls.Add(dgDevices);
            gbDevice.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbDevice.ForeColor = SystemColors.HighlightText;
            gbDevice.Location = new Point(-4, 1);
            gbDevice.Name = "gbDevice";
            gbDevice.Size = new Size(485, 154);
            gbDevice.TabIndex = 3;
            gbDevice.TabStop = false;
            gbDevice.Text = "Device Infomation*";
            gbDevice.Visible = false;
            // 
            // dgDevices
            // 
            dgDevices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgDevices.Location = new Point(11, 13);
            dgDevices.Name = "dgDevices";
            dataGridViewCellStyle2.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dgDevices.RowsDefaultCellStyle = dataGridViewCellStyle2;
            dgDevices.Size = new Size(464, 147);
            dgDevices.TabIndex = 0;
            // 
            // EZcashWedgeConfigurator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSlateGray;
            ClientSize = new Size(505, 499);
            Controls.Add(tbwedgeType);
            Controls.Add(button1);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "EZcashWedgeConfigurator";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EZCash Wedge configurator";
            Load += EZcashWedgeConfigurator_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tbwedgeType.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            gbyard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgYards).EndInit();
            tabPage2.ResumeLayout(false);
            gbDevice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgDevices).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox txtWedgeIp;
        private Label label1;
        private TextBox txtEZCashToken;
        private Label label3;
        private TextBox txtEZCashAPI;
        private Label label2;
        private Button btnSave;
        private Button btnCancel;
        private Button btnConnectEZCashAPI;
        private ToolTip toolTipSaveConfiguration;
        private ToolTip toolTipCancel;
        private ToolTip toolTipTestAPI;
        private Button button1;
        private ToolTip toolTipOpenConfigFile;
        private TextBox txtArchiveRollOutDays;
        private Label label5;
        private TextBox txtTraceSize;
        private Label label4;
        private ComboBox cbWedgeType;
        private Label label6;
        private TabControl tbwedgeType;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox gbDevice;
        private DataGridView dgDevices;
        private GroupBox gbyard;
        private DataGridView dgYards;
    }
}
