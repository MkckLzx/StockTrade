using System.Drawing;
using System.Windows.Forms;
using System.Linq;

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
            
            // 初始化事件
            InitializeEvents();
            
            // 加载配置到UI
            LoadConfig();
        }
        
        /// <summary>
        /// 初始化事件处理
        /// </summary>
        private void InitializeEvents()
        {
            // 为所有颜色选择按钮绑定统一的点击事件
            btnBgColor.Click += ColorButton_Click;
            btnNameColor.Click += ColorButton_Click;
            btnCodeColor.Click += ColorButton_Click;
            btnCurrentPriceColor.Click += ColorButton_Click;
            btnOpenPriceColor.Click += ColorButton_Click;
            btnChangeAmountColor.Click += ColorButton_Click;
            btnChangePercentColor.Click += ColorButton_Click;
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
        /// 颜色选择按钮通用点击事件
        /// </summary>
        private void ColorButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.Color = btn.BackColor;
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        btn.BackColor = colorDialog.Color;
                    }
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
            cmbFontFamily = new ComboBox();
            nudFontSize = new NumericUpDown();
            nudScrollInterval = new NumericUpDown();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            ((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkBgOpacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFontSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudScrollInterval).BeginInit();
            SuspendLayout();
            // 
            // btnBgColor
            // 
            btnBgColor.Location = new Point(236, 42);
            btnBgColor.Margin = new Padding(5, 4, 5, 4);
            btnBgColor.Name = "btnBgColor";
            btnBgColor.Size = new Size(157, 42);
            btnBgColor.TabIndex = 0;
            btnBgColor.Text = "选择颜色";
            btnBgColor.UseVisualStyleBackColor = true;
            btnBgColor.Click += ColorButton_Click;
            // 
            // nudWidth
            // 
            nudWidth.Location = new Point(236, 169);
            nudWidth.Margin = new Padding(5, 4, 5, 4);
            nudWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudWidth.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudWidth.Name = "nudWidth";
            nudWidth.Size = new Size(157, 30);
            nudWidth.TabIndex = 2;
            nudWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudHeight
            // 
            nudHeight.Location = new Point(236, 216);
            nudHeight.Margin = new Padding(5, 4, 5, 4);
            nudHeight.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudHeight.Name = "nudHeight";
            nudHeight.Size = new Size(157, 30);
            nudHeight.TabIndex = 3;
            nudHeight.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // chkShowName
            // 
            chkShowName.AutoSize = true;
            chkShowName.Location = new Point(47, 291);
            chkShowName.Margin = new Padding(5, 4, 5, 4);
            chkShowName.Name = "chkShowName";
            chkShowName.Size = new Size(108, 28);
            chkShowName.TabIndex = 4;
            chkShowName.Text = "显示名称";
            chkShowName.UseVisualStyleBackColor = true;
            // 
            // chkShowCode
            // 
            chkShowCode.AutoSize = true;
            chkShowCode.Location = new Point(270, 291);
            chkShowCode.Margin = new Padding(5, 4, 5, 4);
            chkShowCode.Name = "chkShowCode";
            chkShowCode.Size = new Size(108, 28);
            chkShowCode.TabIndex = 5;
            chkShowCode.Text = "显示代码";
            chkShowCode.UseVisualStyleBackColor = true;
            // 
            // chkShowCurrentPrice
            // 
            chkShowCurrentPrice.AutoSize = true;
            chkShowCurrentPrice.Location = new Point(47, 333);
            chkShowCurrentPrice.Margin = new Padding(5, 4, 5, 4);
            chkShowCurrentPrice.Name = "chkShowCurrentPrice";
            chkShowCurrentPrice.Size = new Size(108, 28);
            chkShowCurrentPrice.TabIndex = 6;
            chkShowCurrentPrice.Text = "显示现价";
            chkShowCurrentPrice.UseVisualStyleBackColor = true;
            // 
            // chkShowOpenPrice
            // 
            chkShowOpenPrice.AutoSize = true;
            chkShowOpenPrice.Location = new Point(270, 333);
            chkShowOpenPrice.Margin = new Padding(5, 4, 5, 4);
            chkShowOpenPrice.Name = "chkShowOpenPrice";
            chkShowOpenPrice.Size = new Size(126, 28);
            chkShowOpenPrice.TabIndex = 7;
            chkShowOpenPrice.Text = "显示开盘价";
            chkShowOpenPrice.UseVisualStyleBackColor = true;
            // 
            // chkShowChangeAmount
            // 
            chkShowChangeAmount.AutoSize = true;
            chkShowChangeAmount.Location = new Point(47, 376);
            chkShowChangeAmount.Margin = new Padding(5, 4, 5, 4);
            chkShowChangeAmount.Name = "chkShowChangeAmount";
            chkShowChangeAmount.Size = new Size(126, 28);
            chkShowChangeAmount.TabIndex = 8;
            chkShowChangeAmount.Text = "显示涨跌额";
            chkShowChangeAmount.UseVisualStyleBackColor = true;
            // 
            // chkShowChangePercent
            // 
            chkShowChangePercent.AutoSize = true;
            chkShowChangePercent.Location = new Point(270, 376);
            chkShowChangePercent.Margin = new Padding(5, 4, 5, 4);
            chkShowChangePercent.Name = "chkShowChangePercent";
            chkShowChangePercent.Size = new Size(126, 28);
            chkShowChangePercent.TabIndex = 9;
            chkShowChangePercent.Text = "显示涨跌幅";
            chkShowChangePercent.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(126, 565);
            btnApply.Margin = new Padding(5, 4, 5, 4);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(126, 42);
            btnApply.TabIndex = 29;
            btnApply.Text = "应用";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(283, 565);
            btnClose.Margin = new Padding(5, 4, 5, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(126, 42);
            btnClose.TabIndex = 30;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 49);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(100, 24);
            label1.TabIndex = 18;
            label1.Text = "背景颜色：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(47, 176);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(64, 24);
            label3.TabIndex = 20;
            label3.Text = "宽度：";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(47, 219);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(64, 24);
            label4.TabIndex = 21;
            label4.Text = "高度：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(47, 256);
            label5.Margin = new Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new Size(100, 24);
            label5.TabIndex = 22;
            label5.Text = "显示字段：";
            // 
            // btnNameColor
            // 
            btnNameColor.Location = new Point(193, 287);
            btnNameColor.Margin = new Padding(5, 4, 5, 4);
            btnNameColor.Name = "btnNameColor";
            btnNameColor.Size = new Size(63, 30);
            btnNameColor.TabIndex = 10;
            btnNameColor.Text = "色";
            btnNameColor.UseVisualStyleBackColor = true;
            btnNameColor.Click += ColorButton_Click;
            // 
            // btnCodeColor
            // 
            btnCodeColor.Location = new Point(409, 291);
            btnCodeColor.Margin = new Padding(5, 4, 5, 4);
            btnCodeColor.Name = "btnCodeColor";
            btnCodeColor.Size = new Size(63, 30);
            btnCodeColor.TabIndex = 11;
            btnCodeColor.Text = "色";
            btnCodeColor.UseVisualStyleBackColor = true;
            btnCodeColor.Click += ColorButton_Click;
            // 
            // btnCurrentPriceColor
            // 
            btnCurrentPriceColor.Location = new Point(193, 329);
            btnCurrentPriceColor.Margin = new Padding(5, 4, 5, 4);
            btnCurrentPriceColor.Name = "btnCurrentPriceColor";
            btnCurrentPriceColor.Size = new Size(63, 30);
            btnCurrentPriceColor.TabIndex = 12;
            btnCurrentPriceColor.Text = "色";
            btnCurrentPriceColor.UseVisualStyleBackColor = true;
            btnCurrentPriceColor.Click += ColorButton_Click;
            // 
            // btnOpenPriceColor
            // 
            btnOpenPriceColor.Location = new Point(409, 333);
            btnOpenPriceColor.Margin = new Padding(5, 4, 5, 4);
            btnOpenPriceColor.Name = "btnOpenPriceColor";
            btnOpenPriceColor.Size = new Size(63, 30);
            btnOpenPriceColor.TabIndex = 13;
            btnOpenPriceColor.Text = "色";
            btnOpenPriceColor.UseVisualStyleBackColor = true;
            btnOpenPriceColor.Click += ColorButton_Click;
            // 
            // btnChangeAmountColor
            // 
            btnChangeAmountColor.Location = new Point(193, 371);
            btnChangeAmountColor.Margin = new Padding(5, 4, 5, 4);
            btnChangeAmountColor.Name = "btnChangeAmountColor";
            btnChangeAmountColor.Size = new Size(63, 30);
            btnChangeAmountColor.TabIndex = 14;
            btnChangeAmountColor.Text = "色";
            btnChangeAmountColor.UseVisualStyleBackColor = true;
            btnChangeAmountColor.Click += ColorButton_Click;
            // 
            // btnChangePercentColor
            // 
            btnChangePercentColor.Location = new Point(409, 376);
            btnChangePercentColor.Margin = new Padding(5, 4, 5, 4);
            btnChangePercentColor.Name = "btnChangePercentColor";
            btnChangePercentColor.Size = new Size(63, 30);
            btnChangePercentColor.TabIndex = 15;
            btnChangePercentColor.Text = "色";
            btnChangePercentColor.UseVisualStyleBackColor = true;
            btnChangePercentColor.Click += ColorButton_Click;
            // 
            // labelNameColor
            // 
            labelNameColor.Location = new Point(0, 0);
            labelNameColor.Margin = new Padding(5, 0, 5, 0);
            labelNameColor.Name = "labelNameColor";
            labelNameColor.Size = new Size(157, 32);
            labelNameColor.TabIndex = 5;
            // 
            // labelCodeColor
            // 
            labelCodeColor.Location = new Point(0, 0);
            labelCodeColor.Margin = new Padding(5, 0, 5, 0);
            labelCodeColor.Name = "labelCodeColor";
            labelCodeColor.Size = new Size(157, 32);
            labelCodeColor.TabIndex = 4;
            // 
            // labelCurrentPriceColor
            // 
            labelCurrentPriceColor.Location = new Point(0, 0);
            labelCurrentPriceColor.Margin = new Padding(5, 0, 5, 0);
            labelCurrentPriceColor.Name = "labelCurrentPriceColor";
            labelCurrentPriceColor.Size = new Size(157, 32);
            labelCurrentPriceColor.TabIndex = 3;
            // 
            // labelOpenPriceColor
            // 
            labelOpenPriceColor.Location = new Point(0, 0);
            labelOpenPriceColor.Margin = new Padding(5, 0, 5, 0);
            labelOpenPriceColor.Name = "labelOpenPriceColor";
            labelOpenPriceColor.Size = new Size(157, 32);
            labelOpenPriceColor.TabIndex = 2;
            // 
            // labelChangeAmountColor
            // 
            labelChangeAmountColor.Location = new Point(0, 0);
            labelChangeAmountColor.Margin = new Padding(5, 0, 5, 0);
            labelChangeAmountColor.Name = "labelChangeAmountColor";
            labelChangeAmountColor.Size = new Size(157, 32);
            labelChangeAmountColor.TabIndex = 1;
            // 
            // labelChangePercentColor
            // 
            labelChangePercentColor.Location = new Point(0, 0);
            labelChangePercentColor.Margin = new Padding(5, 0, 5, 0);
            labelChangePercentColor.Name = "labelChangePercentColor";
            labelChangePercentColor.Size = new Size(157, 32);
            labelChangePercentColor.TabIndex = 0;
            // 
            // trkBgOpacity
            // 
            trkBgOpacity.Location = new Point(236, 99);
            trkBgOpacity.Margin = new Padding(5, 4, 5, 4);
            trkBgOpacity.Maximum = 100;
            trkBgOpacity.Name = "trkBgOpacity";
            trkBgOpacity.Size = new Size(157, 69);
            trkBgOpacity.TabIndex = 1;
            trkBgOpacity.Value = 90;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(47, 120);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(118, 24);
            label6.TabIndex = 19;
            label6.Text = "背景透明度：";
            // 
            // cmbFontFamily
            // 
            cmbFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFontFamily.Location = new Point(236, 412);
            cmbFontFamily.Margin = new Padding(5, 4, 5, 4);
            cmbFontFamily.Name = "cmbFontFamily";
            cmbFontFamily.Size = new Size(155, 32);
            cmbFontFamily.TabIndex = 24;
            // 
            // nudFontSize
            // 
            nudFontSize.Location = new Point(236, 460);
            nudFontSize.Margin = new Padding(5, 4, 5, 4);
            nudFontSize.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudFontSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            nudFontSize.Name = "nudFontSize";
            nudFontSize.Size = new Size(157, 30);
            nudFontSize.TabIndex = 26;
            nudFontSize.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // nudScrollInterval
            // 
            nudScrollInterval.Location = new Point(236, 508);
            nudScrollInterval.Margin = new Padding(5, 4, 5, 4);
            nudScrollInterval.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudScrollInterval.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudScrollInterval.Name = "nudScrollInterval";
            nudScrollInterval.Size = new Size(157, 30);
            nudScrollInterval.TabIndex = 28;
            nudScrollInterval.Value = new decimal(new int[] { 3000, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(47, 418);
            label7.Margin = new Padding(5, 0, 5, 0);
            label7.Name = "label7";
            label7.Size = new Size(64, 24);
            label7.TabIndex = 23;
            label7.Text = "字体：";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(47, 466);
            label8.Margin = new Padding(5, 0, 5, 0);
            label8.Name = "label8";
            label8.Size = new Size(100, 24);
            label8.TabIndex = 25;
            label8.Text = "字体大小：";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(47, 514);
            label9.Margin = new Padding(5, 0, 5, 0);
            label9.Name = "label9";
            label9.Size = new Size(100, 24);
            label9.TabIndex = 27;
            label9.Text = "滚动间隔：";
            // 
            // MiniDisplayConfigForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(756, 706);
            Controls.Add(nudScrollInterval);
            Controls.Add(label9);
            Controls.Add(nudFontSize);
            Controls.Add(label8);
            Controls.Add(cmbFontFamily);
            Controls.Add(label7);
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
            Margin = new Padding(5, 4, 5, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MiniDisplayConfigForm";
            Text = "小窗口配置";
            ((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkBgOpacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFontSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudScrollInterval).EndInit();
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