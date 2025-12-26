namespace StockTrade
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 控件声明
        /// </summary>
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveToken;
        private System.Windows.Forms.TextBox txtApiToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvStocks;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Timer timerScroll;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnAddStock;
        private System.Windows.Forms.TextBox txtStockCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkEnableScrolling;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbApiLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudUpdateInterval;
        private System.Windows.Forms.Button btnApplyInterval;
        private System.Windows.Forms.CheckBox chkEnableMiniDisplay;
        private System.Windows.Forms.CheckBox chkMiniDisplayTopMost;
        private System.Windows.Forms.Button btnMiniConfig;
        private System.Windows.Forms.Button btnHotkeyConfig;

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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            btnHotkeyConfig = new Button();
            chkMiniDisplayTopMost = new CheckBox();
            btnMiniConfig = new Button();
            chkEnableMiniDisplay = new CheckBox();
            btnApplyInterval = new Button();
            nudUpdateInterval = new NumericUpDown();
            label3 = new Label();
            btnExit = new Button();
            chkEnableScrolling = new CheckBox();
            btnAddStock = new Button();
            btnRefresh = new Button();
            txtStockCode = new TextBox();
            label2 = new Label();
            btnSaveToken = new Button();
            txtApiToken = new TextBox();
            label1 = new Label();
            dgvStocks = new DataGridView();
            statusStrip1 = new StatusStrip();
            tsStatusLabel = new ToolStripStatusLabel();
            timerUpdate = new System.Windows.Forms.Timer(components);
            timerScroll = new System.Windows.Forms.Timer(components);
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            showWindowToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            rtbApiLog = new RichTextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudUpdateInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvStocks).BeginInit();
            statusStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnHotkeyConfig);
            panel1.Controls.Add(chkMiniDisplayTopMost);
            panel1.Controls.Add(btnMiniConfig);
            panel1.Controls.Add(chkEnableMiniDisplay);
            panel1.Controls.Add(btnApplyInterval);
            panel1.Controls.Add(nudUpdateInterval);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnExit);
            panel1.Controls.Add(chkEnableScrolling);
            panel1.Controls.Add(btnAddStock);
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(txtStockCode);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(btnSaveToken);
            panel1.Controls.Add(txtApiToken);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 148);
            panel1.TabIndex = 0;
            // 
            // btnHotkeyConfig
            // 
            btnHotkeyConfig.Location = new Point(488, 111);
            btnHotkeyConfig.Name = "btnHotkeyConfig";
            btnHotkeyConfig.Size = new Size(120, 26);
            btnHotkeyConfig.TabIndex = 14;
            btnHotkeyConfig.Text = "快捷键配置";
            btnHotkeyConfig.UseVisualStyleBackColor = true;
            btnHotkeyConfig.Click += btnHotkeyConfig_Click;
            // 
            // chkMiniDisplayTopMost
            // 
            chkMiniDisplayTopMost.AutoSize = true;
            chkMiniDisplayTopMost.Location = new Point(388, 86);
            chkMiniDisplayTopMost.Name = "chkMiniDisplayTopMost";
            chkMiniDisplayTopMost.Size = new Size(111, 21);
            chkMiniDisplayTopMost.TabIndex = 12;
            chkMiniDisplayTopMost.Text = "小窗口置顶显示";
            chkMiniDisplayTopMost.UseVisualStyleBackColor = true;
            chkMiniDisplayTopMost.CheckedChanged += chkMiniDisplayTopMost_CheckedChanged;
            // 
            // btnMiniConfig
            // 
            btnMiniConfig.Location = new Point(628, 111);
            btnMiniConfig.Name = "btnMiniConfig";
            btnMiniConfig.Size = new Size(120, 26);
            btnMiniConfig.TabIndex = 13;
            btnMiniConfig.Text = "小窗配置";
            btnMiniConfig.UseVisualStyleBackColor = true;
            btnMiniConfig.Click += btnMiniConfig_Click;
            // 
            // chkEnableMiniDisplay
            // 
            chkEnableMiniDisplay.AutoSize = true;
            chkEnableMiniDisplay.Location = new Point(278, 86);
            chkEnableMiniDisplay.Name = "chkEnableMiniDisplay";
            chkEnableMiniDisplay.Size = new Size(111, 21);
            chkEnableMiniDisplay.TabIndex = 11;
            chkEnableMiniDisplay.Text = "开启小窗口显示";
            chkEnableMiniDisplay.UseVisualStyleBackColor = true;
            chkEnableMiniDisplay.CheckedChanged += chkEnableMiniDisplay_CheckedChanged;
            // 
            // btnApplyInterval
            // 
            btnApplyInterval.Location = new Point(673, 80);
            btnApplyInterval.Name = "btnApplyInterval";
            btnApplyInterval.Size = new Size(75, 26);
            btnApplyInterval.TabIndex = 10;
            btnApplyInterval.Text = "应用";
            btnApplyInterval.UseVisualStyleBackColor = true;
            btnApplyInterval.Click += btnApplyInterval_Click;
            // 
            // nudUpdateInterval
            // 
            nudUpdateInterval.Location = new Point(593, 82);
            nudUpdateInterval.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudUpdateInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudUpdateInterval.Name = "nudUpdateInterval";
            nudUpdateInterval.Size = new Size(70, 23);
            nudUpdateInterval.TabIndex = 9;
            nudUpdateInterval.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(498, 86);
            label3.Name = "label3";
            label3.Size = new Size(88, 17);
            label3.TabIndex = 8;
            label3.Text = "刷新间隔(秒)：";
            // 
            // btnExit
            // 
            btnExit.Location = new Point(150, 52);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 26);
            btnExit.TabIndex = 7;
            btnExit.Text = "退出程序";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // chkEnableScrolling
            // 
            chkEnableScrolling.AutoSize = true;
            chkEnableScrolling.Location = new Point(12, 56);
            chkEnableScrolling.Name = "chkEnableScrolling";
            chkEnableScrolling.Size = new Size(135, 21);
            chkEnableScrolling.TabIndex = 6;
            chkEnableScrolling.Text = "开启任务栏滚动显示";
            chkEnableScrolling.UseVisualStyleBackColor = true;
            chkEnableScrolling.CheckedChanged += chkEnableScrolling_CheckedChanged;
            // 
            // btnAddStock
            // 
            btnAddStock.Location = new Point(592, 52);
            btnAddStock.Name = "btnAddStock";
            btnAddStock.Size = new Size(75, 26);
            btnAddStock.TabIndex = 5;
            btnAddStock.Text = "添加";
            btnAddStock.UseVisualStyleBackColor = true;
            btnAddStock.Click += btnAddStock_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(673, 52);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 26);
            btnRefresh.TabIndex = 6;
            btnRefresh.Text = "刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtStockCode
            // 
            txtStockCode.Location = new Point(307, 52);
            txtStockCode.Name = "txtStockCode";
            txtStockCode.PlaceholderText = "输入股票/基金代码，多个用逗号分隔";
            txtStockCode.Size = new Size(280, 23);
            txtStockCode.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(227, 56);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 3;
            label2.Text = "股票代码：";
            // 
            // btnSaveToken
            // 
            btnSaveToken.Location = new Point(673, 19);
            btnSaveToken.Name = "btnSaveToken";
            btnSaveToken.Size = new Size(75, 26);
            btnSaveToken.TabIndex = 2;
            btnSaveToken.Text = "保存";
            btnSaveToken.UseVisualStyleBackColor = true;
            btnSaveToken.Click += btnSaveToken_Click;
            // 
            // txtApiToken
            // 
            txtApiToken.Location = new Point(100, 19);
            txtApiToken.Name = "txtApiToken";
            txtApiToken.PlaceholderText = "输入iTick API Token";
            txtApiToken.Size = new Size(567, 23);
            txtApiToken.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 23);
            label1.Name = "label1";
            label1.Size = new Size(79, 17);
            label1.TabIndex = 0;
            label1.Text = "API Token：";
            // 
            // dgvStocks
            // 
            dgvStocks.AllowUserToAddRows = false;
            dgvStocks.AllowUserToDeleteRows = false;
            dgvStocks.AllowUserToOrderColumns = true;
            dgvStocks.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvStocks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvStocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStocks.Dock = DockStyle.Fill;
            dgvStocks.Location = new Point(0, 148);
            dgvStocks.Name = "dgvStocks";
            dgvStocks.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvStocks.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvStocks.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvStocks.RowsDefaultCellStyle = dataGridViewCellStyle3;
            dgvStocks.Size = new Size(800, 260);
            dgvStocks.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsStatusLabel });
            statusStrip1.Location = new Point(0, 525);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsStatusLabel
            // 
            tsStatusLabel.Name = "tsStatusLabel";
            tsStatusLabel.Size = new Size(785, 17);
            tsStatusLabel.Spring = true;
            tsStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // timerUpdate
            // 
            timerUpdate.Interval = 30000;
            timerUpdate.Tick += timerUpdate_Tick;
            // 
            // timerScroll
            // 
            timerScroll.Tick += timerScroll_Tick;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "StockTrade";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { showWindowToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(125, 48);
            // 
            // showWindowToolStripMenuItem
            // 
            showWindowToolStripMenuItem.Name = "showWindowToolStripMenuItem";
            showWindowToolStripMenuItem.Size = new Size(124, 22);
            showWindowToolStripMenuItem.Text = "显示窗口";
            showWindowToolStripMenuItem.Click += showWindowToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(124, 22);
            exitToolStripMenuItem.Text = "退出";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // rtbApiLog
            // 
            rtbApiLog.BackColor = Color.Black;
            rtbApiLog.Dock = DockStyle.Bottom;
            rtbApiLog.Font = new Font("Consolas", 9F);
            rtbApiLog.ForeColor = Color.Lime;
            rtbApiLog.Location = new Point(0, 408);
            rtbApiLog.Name = "rtbApiLog";
            rtbApiLog.ReadOnly = true;
            rtbApiLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbApiLog.Size = new Size(800, 117);
            rtbApiLog.TabIndex = 3;
            rtbApiLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 547);
            Controls.Add(dgvStocks);
            Controls.Add(panel1);
            Controls.Add(rtbApiLog);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "s";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudUpdateInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvStocks).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
