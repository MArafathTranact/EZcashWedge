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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EZcashWedgeConfigurator));
            groupBox1 = new GroupBox();
            btnConnectEZCashAPI = new Button();
            txtWedgeIp = new TextBox();
            label1 = new Label();
            txtEZCashToken = new TextBox();
            label3 = new Label();
            txtEZCashAPI = new TextBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            dgYards = new DataGridView();
            btnSave = new Button();
            btnCancel = new Button();
            toolTipSaveConfiguration = new ToolTip(components);
            toolTipCancel = new ToolTip(components);
            toolTipTestAPI = new ToolTip(components);
            button1 = new Button();
            toolTipOpenConfigFile = new ToolTip(components);
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgYards).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Transparent;
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
            groupBox1.Size = new Size(422, 146);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "EZcash";
            // 
            // btnConnectEZCashAPI
            // 
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
            txtWedgeIp.Font = new Font("Verdana", 8.25F);
            txtWedgeIp.ForeColor = SystemColors.ActiveCaptionText;
            txtWedgeIp.Location = new Point(131, 23);
            txtWedgeIp.Name = "txtWedgeIp";
            txtWedgeIp.Size = new Size(146, 21);
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
            // groupBox2
            // 
            groupBox2.BackColor = Color.Transparent;
            groupBox2.Controls.Add(dgYards);
            groupBox2.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.ForeColor = SystemColors.HighlightText;
            groupBox2.Location = new Point(13, 173);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(485, 154);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Yard Infomation*";
            // 
            // dgYards
            // 
            dgYards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgYards.Location = new Point(11, 19);
            dgYards.Name = "dgYards";
            dataGridViewCellStyle1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dgYards.RowsDefaultCellStyle = dataGridViewCellStyle1;
            dgYards.Size = new Size(464, 124);
            dgYards.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.SeaGreen;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = SystemColors.Desktop;
            btnSave.Location = new Point(109, 340);
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
            btnCancel.Location = new Point(208, 340);
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
            button1.Location = new Point(309, 340);
            button1.Name = "button1";
            button1.Size = new Size(75, 30);
            button1.TabIndex = 4;
            toolTipOpenConfigFile.SetToolTip(button1, "Load EZCashWedge.exe.config file");
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // EZcashWedgeConfigurator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSlateGray;
            ClientSize = new Size(505, 388);
            Controls.Add(button1);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "EZcashWedgeConfigurator";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EZCash Wedge configurator";
            Load += EZcashWedgeConfigurator_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgYards).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox txtWedgeIp;
        private Label label1;
        private TextBox txtEZCashToken;
        private Label label3;
        private TextBox txtEZCashAPI;
        private Label label2;
        private Button btnSave;
        private Button btnCancel;
        private Button btnConnectEZCashAPI;
        private DataGridView dgYards;
        private ToolTip toolTipSaveConfiguration;
        private ToolTip toolTipCancel;
        private ToolTip toolTipTestAPI;
        private Button button1;
        private ToolTip toolTipOpenConfigFile;
    }
}
