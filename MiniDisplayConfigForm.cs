using System.Drawing;
using System.Windows.Forms;

namespace StockTrade
{
    /// <summary>
    /// 小窗口配置窗体
    /// </summary>
    public partial class MiniDisplayConfigForm : Form
    {
        private AppConfig _config;
        
        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event EventHandler ConfigChanged;
        
        public MiniDisplayConfigForm(AppConfig config)
        {
            InitializeComponent();
            _config = config;
            
            // 初始化新增的控件
            cmbFontFamily = new ComboBox();
            nudFontSize = new NumericUpDown();
            nudScrollInterval = new NumericUpDown();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            
            // 设置控件的基本属性
            cmbFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFontFamily.Location = new Point(150, 296);
            cmbFontFamily.Name = "cmbFontFamily";
            cmbFontFamily.Size = new Size(100, 25);
            cmbFontFamily.TabIndex = 23;
            
            nudFontSize.Minimum = 6;
            nudFontSize.Maximum = 20;
            nudFontSize.Location = new Point(150, 330);
            nudFontSize.Name = "nudFontSize";
            nudFontSize.Size = new Size(100, 23);
            nudFontSize.TabIndex = 24;
            nudFontSize.Value = 10;
            
            nudScrollInterval.Minimum = 1000;
            nudScrollInterval.Maximum = 10000;
            nudScrollInterval.Location = new Point(150, 363);
            nudScrollInterval.Name = "nudScrollInterval";
            nudScrollInterval.Size = new Size(100, 23);
            nudScrollInterval.TabIndex = 25;
            nudScrollInterval.Value = 3000;
            
            label7.AutoSize = true;
            label7.Location = new Point(30, 300);
            label7.Name = "label7";
            label7.Size = new Size(44, 17);
            label7.TabIndex = 26;
            label7.Text = "字体：";
            
            label8.AutoSize = true;
            label8.Location = new Point(30, 332);
            label8.Name = "label8";
            label8.Size = new Size(68, 17);
            label8.TabIndex = 27;
            label8.Text = "字体大小：";
            
            label9.AutoSize = true;
            label9.Location = new Point(30, 365);
            label9.Name = "label9";
            label9.Size = new Size(80, 17);
            label9.TabIndex = 28;
            label9.Text = "滚动间隔：";
            
            // 添加控件到表单
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(cmbFontFamily);
            Controls.Add(nudFontSize);
            Controls.Add(nudScrollInterval);
            
            // 调整表单大小
            this.ClientSize = new Size(320, 470);
            if (btnApply != null && btnClose != null)
            {
                btnApply.Location = new Point(80, 420);
                btnClose.Location = new Point(180, 420);
            }
            
            LoadConfig();
        }
        
        /// <summary>
        /// 加载配置到UI
        /// </summary>
        private void LoadConfig()
        {
            // 加载颜色配置
            btnBgColor.BackColor = ParseColor(_config.MiniDisplayBgColor);
            btnNameColor.BackColor = ParseColor(_config.MiniDisplayNameColor);
            btnCodeColor.BackColor = ParseColor(_config.MiniDisplayCodeColor);
            btnCurrentPriceColor.BackColor = ParseColor(_config.MiniDisplayCurrentPriceColor);
            btnOpenPriceColor.BackColor = ParseColor(_config.MiniDisplayOpenPriceColor);
            btnChangeAmountColor.BackColor = ParseColor(_config.MiniDisplayChangeAmountColor);
            btnChangePercentColor.BackColor = ParseColor(_config.MiniDisplayChangePercentColor);
            
            // 加载透明度配置
            trkBgOpacity.Value = _config.MiniDisplayBgOpacity;
            
            // 加载尺寸配置
            nudWidth.Value = _config.MiniDisplayWidth;
            nudHeight.Value = _config.MiniDisplayHeight;
            
            // 加载字体配置
            cmbFontFamily.Items.AddRange(System.Drawing.FontFamily.Families.Select(f => f.Name).ToArray());
            cmbFontFamily.Text = _config.MiniDisplayFontFamily;
            nudFontSize.Value = (decimal)_config.MiniDisplayFontSize;
            nudScrollInterval.Value = _config.MiniDisplayScrollInterval;
            
            // 加载显示字段配置
            chkShowName.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.Name) != 0;
            chkShowCode.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.Code) != 0;
            chkShowCurrentPrice.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.CurrentPrice) != 0;
            chkShowOpenPrice.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.OpenPrice) != 0;
            chkShowChangeAmount.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.ChangeAmount) != 0;
            chkShowChangePercent.Checked = (_config.MiniDisplayShowFields & MiniDisplayFields.ChangePercent) != 0;
        }
        
        /// <summary>
        /// 解析RGB颜色字符串
        /// </summary>
        private Color ParseColor(string colorStr)
        {
            try
            {
                string[] rgb = colorStr.Split(',');
                if (rgb.Length == 3 &&
                    int.TryParse(rgb[0], out int r) &&
                    int.TryParse(rgb[1], out int g) &&
                    int.TryParse(rgb[2], out int b))
                {
                    return Color.FromArgb(r, g, b);
                }
            }
            catch {}
            
            return Color.Black;
        }
        
        /// <summary>
        /// 将Color转换为RGB字符串
        /// </summary>
        private string ColorToString(Color color)
        {
            return $"{color.R},{color.G},{color.B}";
        }
        
        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            // 保存颜色配置
            _config.MiniDisplayBgColor = ColorToString(btnBgColor.BackColor);
            _config.MiniDisplayNameColor = ColorToString(btnNameColor.BackColor);
            _config.MiniDisplayCodeColor = ColorToString(btnCodeColor.BackColor);
            _config.MiniDisplayCurrentPriceColor = ColorToString(btnCurrentPriceColor.BackColor);
            _config.MiniDisplayOpenPriceColor = ColorToString(btnOpenPriceColor.BackColor);
            _config.MiniDisplayChangeAmountColor = ColorToString(btnChangeAmountColor.BackColor);
            _config.MiniDisplayChangePercentColor = ColorToString(btnChangePercentColor.BackColor);
            
            // 保存透明度配置
            _config.MiniDisplayBgOpacity = trkBgOpacity.Value;
            
            // 保存尺寸配置
            _config.MiniDisplayWidth = (int)nudWidth.Value;
            _config.MiniDisplayHeight = (int)nudHeight.Value;
            
            // 保存字体配置
            _config.MiniDisplayFontFamily = cmbFontFamily.Text;
            _config.MiniDisplayFontSize = (float)nudFontSize.Value;
            _config.MiniDisplayScrollInterval = (int)nudScrollInterval.Value;
            
            // 保存显示字段配置
            MiniDisplayFields showFields = MiniDisplayFields.None;
            if (chkShowName.Checked) showFields |= MiniDisplayFields.Name;
            if (chkShowCode.Checked) showFields |= MiniDisplayFields.Code;
            if (chkShowCurrentPrice.Checked) showFields |= MiniDisplayFields.CurrentPrice;
            if (chkShowOpenPrice.Checked) showFields |= MiniDisplayFields.OpenPrice;
            if (chkShowChangeAmount.Checked) showFields |= MiniDisplayFields.ChangeAmount;
            if (chkShowChangePercent.Checked) showFields |= MiniDisplayFields.ChangePercent;
            
            _config.MiniDisplayShowFields = showFields;
            
            // 触发配置变更事件
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// 背景颜色选择按钮点击事件
        /// </summary>
        private void btnBgColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnBgColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnBgColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 名称颜色选择按钮点击事件
        /// </summary>
        private void btnNameColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnNameColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnNameColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 代码颜色选择按钮点击事件
        /// </summary>
        private void btnCodeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnCodeColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnCodeColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 现价颜色选择按钮点击事件
        /// </summary>
        private void btnCurrentPriceColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnCurrentPriceColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnCurrentPriceColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 开盘价颜色选择按钮点击事件
        /// </summary>
        private void btnOpenPriceColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnOpenPriceColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnOpenPriceColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 涨跌额颜色选择按钮点击事件
        /// </summary>
        private void btnChangeAmountColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnChangeAmountColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnChangeAmountColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 涨跌幅颜色选择按钮点击事件
        /// </summary>
        private void btnChangePercentColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = btnChangePercentColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    btnChangePercentColor.BackColor = colorDialog.Color;
                }
            }
        }
        
        /// <summary>
        /// 应用按钮点击事件
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }
        
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            btnBgColor = new Button();
            nudWidth = new NumericUpDown();
            nudHeight = new NumericUpDown();
            chkShowName = new CheckBox();
            chkShowCode = new CheckBox();
            chkShowCurrentPrice = new CheckBox();
            chkShowOpenPrice = new CheckBox();
            chkShowChangeAmount = new CheckBox();
            chkShowChangePercent = new CheckBox();
            btnApply = new Button();
            btnClose = new Button();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            btnNameColor = new Button();
            btnCodeColor = new Button();
            btnCurrentPriceColor = new Button();
            btnOpenPriceColor = new Button();
            btnChangeAmountColor = new Button();
            btnChangePercentColor = new Button();
            labelNameColor = new Label();
            labelCodeColor = new Label();
            labelCurrentPriceColor = new Label();
            labelOpenPriceColor = new Label();
            labelChangeAmountColor = new Label();
            labelChangePercentColor = new Label();
            trkBgOpacity = new TrackBar();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkBgOpacity).BeginInit();
            SuspendLayout();
            // 
            // btnBgColor
            // 
            btnBgColor.Location = new Point(150, 30);
            btnBgColor.Name = "btnBgColor";
            btnBgColor.Size = new Size(100, 30);
            btnBgColor.TabIndex = 0;
            btnBgColor.Text = "选择颜色";
            btnBgColor.UseVisualStyleBackColor = true;
            btnBgColor.Click += btnBgColor_Click;
            // 
            // nudWidth
            // 
            nudWidth.Location = new Point(150, 120);
            nudWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudWidth.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudWidth.Name = "nudWidth";
            nudWidth.Size = new Size(100, 23);
            nudWidth.TabIndex = 2;
            nudWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudHeight
            // 
            nudHeight.Location = new Point(150, 153);
            nudHeight.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudHeight.Name = "nudHeight";
            nudHeight.Size = new Size(100, 23);
            nudHeight.TabIndex = 3;
            nudHeight.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // chkShowName
            // 
            chkShowName.AutoSize = true;
            chkShowName.Location = new Point(30, 206);
            chkShowName.Name = "chkShowName";
            chkShowName.Size = new Size(75, 21);
            chkShowName.TabIndex = 4;
            chkShowName.Text = "显示名称";
            chkShowName.UseVisualStyleBackColor = true;
            // 
            // chkShowCode
            // 
            chkShowCode.AutoSize = true;
            chkShowCode.Location = new Point(172, 206);
            chkShowCode.Name = "chkShowCode";
            chkShowCode.Size = new Size(75, 21);
            chkShowCode.TabIndex = 5;
            chkShowCode.Text = "显示代码";
            chkShowCode.UseVisualStyleBackColor = true;
            // 
            // chkShowCurrentPrice
            // 
            chkShowCurrentPrice.AutoSize = true;
            chkShowCurrentPrice.Location = new Point(30, 236);
            chkShowCurrentPrice.Name = "chkShowCurrentPrice";
            chkShowCurrentPrice.Size = new Size(75, 21);
            chkShowCurrentPrice.TabIndex = 6;
            chkShowCurrentPrice.Text = "显示现价";
            chkShowCurrentPrice.UseVisualStyleBackColor = true;
            // 
            // chkShowOpenPrice
            // 
            chkShowOpenPrice.AutoSize = true;
            chkShowOpenPrice.Location = new Point(172, 236);
            chkShowOpenPrice.Name = "chkShowOpenPrice";
            chkShowOpenPrice.Size = new Size(87, 21);
            chkShowOpenPrice.TabIndex = 7;
            chkShowOpenPrice.Text = "显示开盘价";
            chkShowOpenPrice.UseVisualStyleBackColor = true;
            // 
            // chkShowChangeAmount
            // 
            chkShowChangeAmount.AutoSize = true;
            chkShowChangeAmount.Location = new Point(30, 266);
            chkShowChangeAmount.Name = "chkShowChangeAmount";
            chkShowChangeAmount.Size = new Size(87, 21);
            chkShowChangeAmount.TabIndex = 8;
            chkShowChangeAmount.Text = "显示涨跌额";
            chkShowChangeAmount.UseVisualStyleBackColor = true;
            // 
            // chkShowChangePercent
            // 
            chkShowChangePercent.AutoSize = true;
            chkShowChangePercent.Location = new Point(172, 266);
            chkShowChangePercent.Name = "chkShowChangePercent";
            chkShowChangePercent.Size = new Size(87, 21);
            chkShowChangePercent.TabIndex = 9;
            chkShowChangePercent.Text = "显示涨跌幅";
            chkShowChangePercent.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(80, 301);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(80, 30);
            btnApply.TabIndex = 16;
            btnApply.Text = "应用";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(180, 300);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 30);
            btnClose.TabIndex = 17;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 35);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 18;
            label1.Text = "背景颜色：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 125);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 20;
            label3.Text = "宽度：";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(30, 155);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 21;
            label4.Text = "高度：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(30, 181);
            label5.Name = "label5";
            label5.Size = new Size(68, 17);
            label5.TabIndex = 22;
            label5.Text = "显示字段：";
            // 
            // btnNameColor
            // 
            btnNameColor.Location = new Point(123, 203);
            btnNameColor.Name = "btnNameColor";
            btnNameColor.Size = new Size(40, 21);
            btnNameColor.TabIndex = 10;
            btnNameColor.Text = "色";
            btnNameColor.UseVisualStyleBackColor = true;
            btnNameColor.Click += btnNameColor_Click;
            // 
            // btnCodeColor
            // 
            btnCodeColor.Location = new Point(260, 206);
            btnCodeColor.Name = "btnCodeColor";
            btnCodeColor.Size = new Size(40, 21);
            btnCodeColor.TabIndex = 11;
            btnCodeColor.Text = "色";
            btnCodeColor.UseVisualStyleBackColor = true;
            btnCodeColor.Click += btnCodeColor_Click;
            // 
            // btnCurrentPriceColor
            // 
            btnCurrentPriceColor.Location = new Point(123, 233);
            btnCurrentPriceColor.Name = "btnCurrentPriceColor";
            btnCurrentPriceColor.Size = new Size(40, 21);
            btnCurrentPriceColor.TabIndex = 12;
            btnCurrentPriceColor.Text = "色";
            btnCurrentPriceColor.UseVisualStyleBackColor = true;
            btnCurrentPriceColor.Click += btnCurrentPriceColor_Click;
            // 
            // btnOpenPriceColor
            // 
            btnOpenPriceColor.Location = new Point(260, 236);
            btnOpenPriceColor.Name = "btnOpenPriceColor";
            btnOpenPriceColor.Size = new Size(40, 21);
            btnOpenPriceColor.TabIndex = 13;
            btnOpenPriceColor.Text = "色";
            btnOpenPriceColor.UseVisualStyleBackColor = true;
            btnOpenPriceColor.Click += btnOpenPriceColor_Click;
            // 
            // btnChangeAmountColor
            // 
            btnChangeAmountColor.Location = new Point(123, 263);
            btnChangeAmountColor.Name = "btnChangeAmountColor";
            btnChangeAmountColor.Size = new Size(40, 21);
            btnChangeAmountColor.TabIndex = 14;
            btnChangeAmountColor.Text = "色";
            btnChangeAmountColor.UseVisualStyleBackColor = true;
            btnChangeAmountColor.Click += btnChangeAmountColor_Click;
            // 
            // btnChangePercentColor
            // 
            btnChangePercentColor.Location = new Point(260, 266);
            btnChangePercentColor.Name = "btnChangePercentColor";
            btnChangePercentColor.Size = new Size(40, 21);
            btnChangePercentColor.TabIndex = 15;
            btnChangePercentColor.Text = "色";
            btnChangePercentColor.UseVisualStyleBackColor = true;
            btnChangePercentColor.Click += btnChangePercentColor_Click;
            // 
            // labelNameColor
            // 
            labelNameColor.Location = new Point(0, 0);
            labelNameColor.Name = "labelNameColor";
            labelNameColor.Size = new Size(100, 23);
            labelNameColor.TabIndex = 5;
            // 
            // labelCodeColor
            // 
            labelCodeColor.Location = new Point(0, 0);
            labelCodeColor.Name = "labelCodeColor";
            labelCodeColor.Size = new Size(100, 23);
            labelCodeColor.TabIndex = 4;
            // 
            // labelCurrentPriceColor
            // 
            labelCurrentPriceColor.Location = new Point(0, 0);
            labelCurrentPriceColor.Name = "labelCurrentPriceColor";
            labelCurrentPriceColor.Size = new Size(100, 23);
            labelCurrentPriceColor.TabIndex = 3;
            // 
            // labelOpenPriceColor
            // 
            labelOpenPriceColor.Location = new Point(0, 0);
            labelOpenPriceColor.Name = "labelOpenPriceColor";
            labelOpenPriceColor.Size = new Size(100, 23);
            labelOpenPriceColor.TabIndex = 2;
            // 
            // labelChangeAmountColor
            // 
            labelChangeAmountColor.Location = new Point(0, 0);
            labelChangeAmountColor.Name = "labelChangeAmountColor";
            labelChangeAmountColor.Size = new Size(100, 23);
            labelChangeAmountColor.TabIndex = 1;
            // 
            // labelChangePercentColor
            // 
            labelChangePercentColor.Location = new Point(0, 0);
            labelChangePercentColor.Name = "labelChangePercentColor";
            labelChangePercentColor.Size = new Size(100, 23);
            labelChangePercentColor.TabIndex = 0;
            // 
            // trkBgOpacity
            // 
            trkBgOpacity.Location = new Point(150, 70);
            trkBgOpacity.Maximum = 100;
            trkBgOpacity.Name = "trkBgOpacity";
            trkBgOpacity.Size = new Size(100, 45);
            trkBgOpacity.TabIndex = 1;
            trkBgOpacity.Value = 90;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(30, 85);
            label6.Name = "label6";
            label6.Size = new Size(80, 17);
            label6.TabIndex = 19;
            label6.Text = "背景透明度：";
            // 
            // MiniDisplayConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 340);
            Controls.Add(labelChangePercentColor);
            Controls.Add(labelChangeAmountColor);
            Controls.Add(labelOpenPriceColor);
            Controls.Add(labelCurrentPriceColor);
            Controls.Add(labelCodeColor);
            Controls.Add(labelNameColor);
            Controls.Add(btnChangePercentColor);
            Controls.Add(btnChangeAmountColor);
            Controls.Add(btnOpenPriceColor);
            Controls.Add(btnCurrentPriceColor);
            Controls.Add(btnCodeColor);
            Controls.Add(btnNameColor);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(label1);
            Controls.Add(btnClose);
            Controls.Add(btnApply);
            Controls.Add(chkShowChangePercent);
            Controls.Add(chkShowChangeAmount);
            Controls.Add(chkShowOpenPrice);
            Controls.Add(chkShowCurrentPrice);
            Controls.Add(chkShowCode);
            Controls.Add(chkShowName);
            Controls.Add(nudHeight);
            Controls.Add(nudWidth);
            Controls.Add(trkBgOpacity);
            Controls.Add(btnBgColor);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MiniDisplayConfigForm";
            Text = "小窗口配置";
            ((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkBgOpacity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button btnBgColor;
        private Button btnNameColor;
        private Button btnCodeColor;
        private Button btnCurrentPriceColor;
        private Button btnOpenPriceColor;
        private Button btnChangeAmountColor;
        private Button btnChangePercentColor;
        private NumericUpDown nudWidth;
        private NumericUpDown nudHeight;
        private CheckBox chkShowName;
        private CheckBox chkShowCode;
        private CheckBox chkShowCurrentPrice;
        private CheckBox chkShowOpenPrice;
        private CheckBox chkShowChangeAmount;
        private CheckBox chkShowChangePercent;
        private Button btnApply;
        private Button btnClose;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label labelNameColor;
        private Label labelCodeColor;
        private Label labelCurrentPriceColor;
        private Label labelOpenPriceColor;
        private Label labelChangeAmountColor;
        private Label labelChangePercentColor;
        private TrackBar trkBgOpacity;
        private ComboBox cmbFontFamily;
        private NumericUpDown nudFontSize;
        private NumericUpDown nudScrollInterval;
    }
}