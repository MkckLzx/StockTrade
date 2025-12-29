using System.Windows.Forms;
using System.Text;

namespace StockTrade
{
    /// <summary>
    /// 全局快捷键配置窗体
    /// </summary>
    public partial class GlobalHotkeyConfigForm : Form
    {
        #region 私有字段

        private string _currentHotkey;
        private bool _isRecording;
        private Keys _hotkey;
        private Keys _modifiers;
        
        #endregion

        #region 事件

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event EventHandler<string>? HotkeyChanged;
        
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currentHotkey">当前快捷键</param>
        public GlobalHotkeyConfigForm(string currentHotkey)
        {
            InitializeComponent();
            _currentHotkey = currentHotkey;
            txtHotkey.Text = currentHotkey;
            ParseHotkey(currentHotkey);
        }
        
        #endregion

        #region 私有方法

        /// <summary>
        /// 解析快捷键字符串
        /// </summary>
        /// <param name="hotkeyStr">快捷键字符串</param>
        private void ParseHotkey(string hotkeyStr)
        {
            if (string.IsNullOrEmpty(hotkeyStr))
            {
                _modifiers = Keys.None;
                _hotkey = Keys.None;
                return;
            }
            
            string[] parts = hotkeyStr.Split('+');
            _modifiers = Keys.None;
            
            for (int i = 0; i < parts.Length - 1; i++)
            {
                switch (parts[i].ToLower())
                {
                    case "ctrl":
                        _modifiers |= Keys.Control;
                        break;
                    case "alt":
                        _modifiers |= Keys.Alt;
                        break;
                    case "shift":
                        _modifiers |= Keys.Shift;
                        break;
                }
            }
            
            if (parts.Length > 0)
            {
                Keys.TryParse(parts[^1], out _hotkey);
            }
        }
        
        /// <summary>
        /// 生成快捷键字符串
        /// </summary>
        /// <returns>快捷键字符串</returns>
        private string GenerateHotkeyString()
        {
            StringBuilder sb = new StringBuilder();
            
            if ((_modifiers & Keys.Control) != 0)
                sb.Append("Ctrl+");
            if ((_modifiers & Keys.Alt) != 0)
                sb.Append("Alt+");
            if ((_modifiers & Keys.Shift) != 0)
                sb.Append("Shift+");
            
            sb.Append(_hotkey.ToString());
            
            return sb.ToString();
        }
        
        #endregion

        #region 事件处理

        /// <summary>
        /// 录制快捷键按钮点击事件
        /// </summary>
        private void btnRecord_Click(object sender, EventArgs e)
        {
            _isRecording = true;
            txtHotkey.Text = "请按下组合键...";
            btnRecord.Enabled = false;
        }
        
        /// <summary>
        /// 重置快捷键按钮点击事件
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            _modifiers = Keys.Alt;
            _hotkey = Keys.Q;
            txtHotkey.Text = "Alt+Q";
        }
        
        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            string hotkey = txtHotkey.Text;
            if (hotkey != "请按下组合键...")
            {
                HotkeyChanged?.Invoke(this, hotkey);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        #endregion

        #region 重写方法

        /// <summary>
        /// 重写ProcessCmdKey方法，处理按键录制
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_isRecording)
            {
                // 处理快捷键录制
                _modifiers = keyData & Keys.Modifiers;
                _hotkey = keyData & Keys.KeyCode;
                
                if (_hotkey != Keys.None && _modifiers != Keys.None)
                {
                    txtHotkey.Text = GenerateHotkeyString();
                    _isRecording = false;
                    btnRecord.Enabled = true;
                }
                
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        #endregion
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            this.txtHotkey = new TextBox();
            this.btnRecord = new Button();
            this.btnReset = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.label1 = new Label();
            this.SuspendLayout();
            // 
            // txtHotkey
            // 
            this.txtHotkey.Location = new Point(120, 30);
            this.txtHotkey.Name = "txtHotkey";
            this.txtHotkey.ReadOnly = true;
            this.txtHotkey.Size = new Size(100, 25);
            this.txtHotkey.TabIndex = 0;
            this.txtHotkey.TextAlign = HorizontalAlignment.Center;
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new Point(30, 70);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new Size(90, 26);
            this.btnRecord.TabIndex = 1;
            this.btnRecord.Text = "录制快捷键";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += this.btnRecord_Click;
            // 
            // btnReset
            // 
            this.btnReset.Location = new Point(130, 70);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new Size(90, 26);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "重置默认";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += this.btnReset_Click;
            // 
            // btnOK
            // 
            this.btnOK.Location = new Point(50, 110);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(70, 26);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += this.btnOK_Click;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new Point(130, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(70, 26);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += this.btnCancel_Click;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(30, 35);
            this.label1.Name = "label1";
            this.label1.Size = new Size(82, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "全局快捷键：";
            // 
            // GlobalHotkeyConfigForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(240, 150);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.txtHotkey);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalHotkeyConfigForm";
            this.Text = "快捷键配置";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private TextBox txtHotkey;
        private Button btnRecord;
        private Button btnReset;
        private Button btnOK;
        private Button btnCancel;
        private Label label1;
    }
}