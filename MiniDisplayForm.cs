using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace StockTrade
{
    /// <summary>
    /// 小窗口展示类
    /// </summary>
    public partial class MiniDisplayForm : Form
    {
        // Windows API常量和函数声明，用于确保窗口始终在最顶层
        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int HWND_DESKTOP = 0;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;
        private const int WM_ACTIVATE = 0x0006;
        private const int WA_ACTIVE = 1;
        private const int WA_INACTIVE = 0;
        
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        
        private List<Stock> _stockList; // 要显示的股票列表
        private int _currentStockIndex; // 当前显示的股票索引
        private bool _isDragging;
        private Point _dragStartPoint;
        private System.Windows.Forms.Timer _topMostTimer; // 用于定期检查最顶层状态
        private System.Windows.Forms.Timer _scrollTimer; // 用于控制滚动显示
        private AppConfig _currentConfig; // 当前配置
        
        public MiniDisplayForm(AppConfig config = null)
        {
            // 在窗口创建前设置扩展样式
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.9;
            
            InitializeComponent();
            
            // 初始化股票列表
            _stockList = new List<Stock>();
            _currentStockIndex = 0;
            
            // 启用鼠标拖动事件
            this.MouseDown += MiniDisplayForm_MouseDown;
            this.MouseMove += MiniDisplayForm_MouseMove;
            this.MouseUp += MiniDisplayForm_MouseUp;
            
            // 为所有标签添加鼠标事件，使整个窗口都可以拖动
            AddMouseEvents(lblName);
            AddMouseEvents(lblCode);
            AddMouseEvents(lblCurrentPrice);
            AddMouseEvents(lblOpenPrice);
            AddMouseEvents(lblChangeAmount);
            AddMouseEvents(lblChangePercent);
            
            // 初始化定时器，定期检查最顶层状态
            _topMostTimer = new System.Windows.Forms.Timer();
            _topMostTimer.Interval = 1000; // 每秒检查一次
            _topMostTimer.Tick += TopMostTimer_Tick;
            _topMostTimer.Start();
            
            // 初始化滚动定时器，控制股票滚动显示
            _scrollTimer = new System.Windows.Forms.Timer();
            _scrollTimer.Interval = 3000; // 默认3秒切换一次
            _scrollTimer.Tick += ScrollTimer_Tick;
            _scrollTimer.Start();
            
            // 应用配置
            if (config != null)
            {
                ApplyConfig(config);
            }
            
            // 将默认位置调整到左下角
            this.Location = new Point(20, Screen.PrimaryScreen.Bounds.Bottom - this.Height - 20);
        }
        
        /// <summary>
        /// 为标签添加鼠标事件
        /// </summary>
        private void AddMouseEvents(Label label)
        {
            label.MouseDown += MiniDisplayForm_MouseDown;
            label.MouseMove += MiniDisplayForm_MouseMove;
            label.MouseUp += MiniDisplayForm_MouseUp;
        }
        
        /// <summary>
        /// 应用配置到窗口
        /// </summary>
        public void ApplyConfig(AppConfig config)
        {
            // 应用尺寸配置
            this.Size = new Size(config.MiniDisplayWidth, config.MiniDisplayHeight);
            
            // 应用背景颜色（不含透明度）
            Color bgColor = ParseColor(config.MiniDisplayBgColor);
            this.BackColor = bgColor;
            
            // 应用背景透明度（使用Opacity属性）
            this.Opacity = config.MiniDisplayBgOpacity / 100.0;
            
            // 应用字体配置
            Font font = new Font(config.MiniDisplayFontFamily, config.MiniDisplayFontSize, FontStyle.Bold);
            lblName.Font = font;
            lblCode.Font = font;
            lblCurrentPrice.Font = font;
            lblOpenPrice.Font = font;
            lblChangeAmount.Font = font;
            lblChangePercent.Font = font;
            
            // 应用滚动时间间隔配置
            _scrollTimer.Interval = config.MiniDisplayScrollInterval;
            
            // 保存当前配置
            _currentConfig = config;
            
            // 更新显示
            UpdateDisplay();
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
        /// 重写OnLoad方法，确保窗口样式在加载时正确设置
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 确保窗口加载后立即应用最顶层样式
            SetTopMostStyle(true);
        }
        
        /// <summary>
        /// 设置窗口是否为最顶层样式
        /// </summary>
        public void SetTopMostStyle(bool isTopMost)
        {
            if (isTopMost)
            {
                // 使用更可靠的方式确保窗口始终在最顶层
                // 1. 先获取当前扩展样式
                int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
                // 2. 添加WS_EX_TOPMOST和WS_EX_NOACTIVATE扩展样式
                exStyle |= WS_EX_TOPMOST | WS_EX_NOACTIVATE;
                SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
                // 3. 使用SetWindowPos确认最顶层位置
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                // 4. 确保窗口保持在最顶层
                KeepAlwaysOnTop();
            }
            else
            {
                // 1. 移除WS_EX_TOPMOST扩展样式
                int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
                exStyle &= ~WS_EX_TOPMOST;
                SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
                // 2. 使用SetWindowPos将窗口移回普通层级
                SetWindowPos(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
            
            // 设置TopMost属性，保持与API设置一致
            this.TopMost = isTopMost;
        }
        
        /// <summary>
        /// 确保窗口始终在最顶层，使用更高级的技术
        /// </summary>
        private void KeepAlwaysOnTop()
        {
            // 获取当前前台窗口
            IntPtr foregroundWindow = GetForegroundWindow();
            
            // 如果当前窗口不是前台窗口，尝试将其设置为前台窗口
            if (foregroundWindow != this.Handle)
            {
                // 获取当前线程ID
                uint currentThreadId = GetCurrentThreadId();
                // 获取前台窗口的线程ID
                uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, out _);
                
                // 附加到前台线程，以便可以设置前台窗口
                AttachThreadInput(currentThreadId, foregroundThreadId, true);
                
                try
                {
                    // 设置当前窗口为前台窗口
                    SetForegroundWindow(this.Handle);
                    // 再次确认窗口为最顶层
                    SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                }
                finally
                {
                    // 分离线程输入
                    AttachThreadInput(currentThreadId, foregroundThreadId, false);
                }
            }
        }
        
        /// <summary>
        /// 重写WndProc方法，处理窗口消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // 处理WM_ACTIVATE消息，确保窗口失去激活时仍保持最顶层
            if (m.Msg == WM_ACTIVATE)
            {
                if (this.TopMost)
                {
                    // 无论激活状态如何，都确保窗口保持最顶层
                    SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                }
            }
            
            base.WndProc(ref m);
        }
        
        /// <summary>
        /// 重写OnActivated方法，确保窗口激活时保持最顶层
        /// </summary>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (this.TopMost)
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }
        
        /// <summary>
        /// 重写OnShown方法，确保窗口显示时保持最顶层
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (this.TopMost)
            {
                // 显示后立即确保窗口在最顶层
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                // 额外调用KeepAlwaysOnTop确保效果
                KeepAlwaysOnTop();
            }
        }
        
        /// <summary>
        /// 定时器Tick事件，定期检查最顶层状态
        /// </summary>
        private void TopMostTimer_Tick(object sender, EventArgs e)
        {
            if (this.TopMost && this.Visible)
            {
                // 定期调用SetWindowPos确保窗口保持在最顶层
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }
        
        /// <summary>
        /// 鼠标按下事件，开始拖动
        /// </summary>
        private void MiniDisplayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
                this.Cursor = Cursors.SizeAll;
            }
        }
        
        /// <summary>
        /// 鼠标移动事件，处理拖动
        /// </summary>
        private void MiniDisplayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.Button == MouseButtons.Left)
            {
                Point newPoint = this.PointToScreen(new Point(e.X, e.Y));
                newPoint.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                this.Location = newPoint;
            }
        }
        
        /// <summary>
        /// 鼠标释放事件，结束拖动
        /// </summary>
        private void MiniDisplayForm_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            this.Cursor = Cursors.Default;
        }
        
        /// <summary>
        /// 设置要显示的股票数据
        /// </summary>
        public void SetStockData(List<Stock> stocks)
        {
            if (stocks != null)
            {
                // 过滤出要在小窗中显示的股票
                _stockList = stocks.Where(stock => stock.ShowInMiniDisplay).ToList();
                
                // 重置当前显示索引
                _currentStockIndex = 0;
                
                // 更新显示
                ShowCurrentStock();
            }
        }
        
        /// <summary>
        /// 显示当前索引对应的股票
        /// </summary>
        private void ShowCurrentStock()
        {
            if (_stockList.Count > 0)
            {
                // 确保索引在有效范围内
                if (_currentStockIndex >= _stockList.Count)
                {
                    _currentStockIndex = 0;
                }
                
                // 更新显示
                UpdateDisplay();
            }
        }
        
        /// <summary>
        /// 滚动定时器Tick事件，切换显示的股票
        /// </summary>
        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            if (_stockList.Count > 1)
            {
                // 切换到下一个股票
                _currentStockIndex++;
                if (_currentStockIndex >= _stockList.Count)
                {
                    _currentStockIndex = 0;
                }
                
                // 更新显示
                ShowCurrentStock();
            }
        }
        
        /// <summary>
        /// 更新显示内容
        /// </summary>
        private void UpdateDisplay()
        {
            if (_stockList.Count > 0 && _currentConfig != null)
            {
                Stock currentStock = _stockList[_currentStockIndex];
                
                // 为每个字段设置文本
                lblName.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.Name) != 0 ? currentStock.Name : "";
                lblCode.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.Code) != 0 ? currentStock.Code : "";
                lblCurrentPrice.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.CurrentPrice) != 0 ? $"现价: {currentStock.CurrentPrice:N4}" : "";
                lblOpenPrice.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.OpenPrice) != 0 ? $"开盘: {currentStock.OpenPrice:N4}" : "";
                lblChangeAmount.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.ChangeAmount) != 0 ? $"涨跌: {currentStock.ChangeAmount:N4}" : "";
                lblChangePercent.Text = (_currentConfig.MiniDisplayShowFields & MiniDisplayFields.ChangePercent) != 0 ? $"{currentStock.ChangePercent:F2}%" : "";
                
                // 应用字段颜色
                ApplyFieldColors();
                
                // 调整控件位置
                AdjustControls();
            }
        }
        
        /// <summary>
        /// 应用字段颜色配置
        /// </summary>
        private void ApplyFieldColors()
        {
            if (_currentConfig == null)
                return;
            
            // 为每个字段设置单独的颜色
            lblName.ForeColor = ParseColor(_currentConfig.MiniDisplayNameColor);
            lblCode.ForeColor = ParseColor(_currentConfig.MiniDisplayCodeColor);
            lblCurrentPrice.ForeColor = ParseColor(_currentConfig.MiniDisplayCurrentPriceColor);
            lblOpenPrice.ForeColor = ParseColor(_currentConfig.MiniDisplayOpenPriceColor);
            lblChangeAmount.ForeColor = ParseColor(_currentConfig.MiniDisplayChangeAmountColor);
            lblChangePercent.ForeColor = ParseColor(_currentConfig.MiniDisplayChangePercentColor);
        }
        
        /// <summary>
        /// 调整控件位置和大小，改为同一行显示
        /// </summary>
        private void AdjustControls()
        {
            int xPosition = 10;
            int yPosition = 10;
            int labelHeight = this.Height - 20;
            int spacing = 5;
            
            // 显示或隐藏每个标签，并调整位置到同一行
            AdjustLabelPosition(lblName, ref xPosition, yPosition, labelHeight, spacing);
            AdjustLabelPosition(lblCode, ref xPosition, yPosition, labelHeight, spacing);
            AdjustLabelPosition(lblCurrentPrice, ref xPosition, yPosition, labelHeight, spacing);
            AdjustLabelPosition(lblOpenPrice, ref xPosition, yPosition, labelHeight, spacing);
            AdjustLabelPosition(lblChangeAmount, ref xPosition, yPosition, labelHeight, spacing);
            AdjustLabelPosition(lblChangePercent, ref xPosition, yPosition, labelHeight, spacing);
        }
        
        /// <summary>
        /// 调整单个标签的位置和可见性，改为同一行显示
        /// </summary>
        private void AdjustLabelPosition(Label label, ref int xPosition, int yPosition, int labelHeight, int spacing)
        {
            if (!string.IsNullOrEmpty(label.Text))
            {
                label.Visible = true;
                label.Location = new Point(xPosition, yPosition);
                // 根据文本长度动态计算宽度，确保不相互覆盖
                label.Size = new Size((int)TextRenderer.MeasureText(label.Text, label.Font).Width + 10, labelHeight);
                xPosition += label.Width + spacing;
            }
            else
            {
                label.Visible = false;
            }
        }
        
        /// <summary>
        /// 双击窗口隐藏
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            this.Hide();
            
            // 触发隐藏事件，通知主窗口更新状态
            Hidden?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// 当窗口隐藏时触发的事件
        /// </summary>
        public event EventHandler Hidden;
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            lblName = new Label();
            lblCode = new Label();
            lblCurrentPrice = new Label();
            lblOpenPrice = new Label();
            lblChangeAmount = new Label();
            lblChangePercent = new Label();
            SuspendLayout();
            
            // lblName
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblName.ForeColor = Color.White;
            lblName.Location = new Point(10, 10);
            lblName.Name = "lblName";
            lblName.Size = new Size(43, 19);
            lblName.Text = "名称";
            
            // lblCode
            lblCode.AutoSize = true;
            lblCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCode.ForeColor = Color.White;
            lblCode.Location = new Point(10, 35);
            lblCode.Name = "lblCode";
            lblCode.Size = new Size(43, 19);
            lblCode.Text = "代码";
            
            // lblCurrentPrice
            lblCurrentPrice.AutoSize = true;
            lblCurrentPrice.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCurrentPrice.ForeColor = Color.White;
            lblCurrentPrice.Location = new Point(10, 60);
            lblCurrentPrice.Name = "lblCurrentPrice";
            lblCurrentPrice.Size = new Size(43, 19);
            lblCurrentPrice.Text = "现价";
            
            // lblOpenPrice
            lblOpenPrice.AutoSize = true;
            lblOpenPrice.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOpenPrice.ForeColor = Color.White;
            lblOpenPrice.Location = new Point(10, 85);
            lblOpenPrice.Name = "lblOpenPrice";
            lblOpenPrice.Size = new Size(43, 19);
            lblOpenPrice.Text = "开盘";
            
            // lblChangeAmount
            lblChangeAmount.AutoSize = true;
            lblChangeAmount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblChangeAmount.ForeColor = Color.White;
            lblChangeAmount.Location = new Point(10, 110);
            lblChangeAmount.Name = "lblChangeAmount";
            lblChangeAmount.Size = new Size(43, 19);
            lblChangeAmount.Text = "涨跌";
            
            // lblChangePercent
            lblChangePercent.AutoSize = true;
            lblChangePercent.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblChangePercent.Location = new Point(10, 135);
            lblChangePercent.Name = "lblChangePercent";
            lblChangePercent.Size = new Size(59, 19);
            lblChangePercent.Text = "涨跌幅";
            
            // MiniDisplayForm
            Controls.Add(lblName);
            Controls.Add(lblCode);
            Controls.Add(lblCurrentPrice);
            Controls.Add(lblOpenPrice);
            Controls.Add(lblChangeAmount);
            Controls.Add(lblChangePercent);
            Name = "MiniDisplayForm";
            ResumeLayout(false);
            PerformLayout();
        }
        
        private Label lblName;
        private Label lblCode;
        private Label lblCurrentPrice;
        private Label lblOpenPrice;
        private Label lblChangeAmount;
        private Label lblChangePercent;
    }
}