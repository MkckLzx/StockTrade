using System.Runtime.InteropServices;
using System.Text.Json;

namespace StockTrade
{
    public partial class Form1 : Form
    {
        #region Windows API 热键相关

        // 注册热键API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        // 注销热键API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 热键消息ID
        private const int WM_HOTKEY = 0x0312;

        // 热键ID
        private const int HOTKEY_ID = 1;

        // 热键修饰符
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;

        #endregion

        #region 私有字段

        private AppConfig _config;
        private ApiClient _apiClient;
        private List<Stock> _stockList;
        private string _configFilePath;
        private int _scrollPosition = 0;
        private string _scrollText = string.Empty;
        private MiniDisplayForm _miniDisplayForm;
        // 添加BindingSource字段，用于管理数据绑定
        private BindingSource _bindingSource;

        #endregion

        #region 构造函数和初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        /// <summary>
        /// 初始化应用程序
        /// </summary>
        private void InitializeApp()
        {
            // 设置配置文件路径
            string appDataPath = Application.LocalUserAppDataPath;
            _configFilePath = Path.Combine(appDataPath, "config.json");

            // 加载配置
            LoadConfig();

            // 初始化股票列表
            _stockList = new List<Stock>();

            // 初始化DataGridView
            InitializeDataGridView();

            // 初始化API客户端
            UpdateApiClient();

            // 初始化小窗口，传递配置
            _miniDisplayForm = new MiniDisplayForm(_config);
            // 绑定小窗口隐藏事件
            _miniDisplayForm.Hidden += (sender, e) =>
            {
                // 更新配置和UI，当小窗口被双击隐藏时
                _config.EnableMiniDisplay = false;
                chkEnableMiniDisplay.Checked = false;
                SaveConfig();
            };
            // 根据配置更新小窗口显示状态和置顶状态
            UpdateMiniDisplayStatus();

            // 绑定Resize事件
            this.Resize += Form1_Resize;

            // 设置定时器间隔
            timerUpdate.Interval = _config.UpdateInterval * 1000;

            // 启动定时任务
            timerUpdate.Start();

            // 根据配置启动或停止滚动定时器
            UpdateScrollTimerStatus();

            // 初始更新数据
            Task.Run(async () => await UpdateStockDataAsync());

            // 生成滚动文本
            UpdateScrollText();
        }

        /// <summary>
        /// 初始化DataGridView
        /// </summary>
        private void InitializeDataGridView()
        {
            // 设置双缓冲，减少闪烁和白屏
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgvStocks, new object[] { true });

            // 设置DataGridView样式和性能优化
            dgvStocks.AutoGenerateColumns = false;
            dgvStocks.ColumnHeadersHeight = 30;
            dgvStocks.RowHeadersVisible = false;
            dgvStocks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 性能优化设置
            dgvStocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvStocks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvStocks.AllowUserToResizeColumns = false;
            dgvStocks.AllowUserToResizeRows = false;
            dgvStocks.AllowUserToOrderColumns = false;
            dgvStocks.AllowUserToAddRows = false;
            dgvStocks.AllowUserToDeleteRows = false;
            dgvStocks.EnableHeadersVisualStyles = false;
            dgvStocks.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvStocks.ScrollBars = ScrollBars.Vertical;

            // 添加股票代码列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "代码",
                DataPropertyName = "Code",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // 添加股票名称列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "名称",
                DataPropertyName = "Name",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            // 添加当前价格列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CurrentPrice",
                HeaderText = "当前价",
                DataPropertyName = "CurrentPrice",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N4" }
            });

            // 添加开盘价格列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OpenPrice",
                HeaderText = "开盘价",
                DataPropertyName = "OpenPrice",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N4" }
            });

            // 添加涨跌金额列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChangeAmount",
                HeaderText = "涨跌",
                DataPropertyName = "ChangeAmount",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N4" }
            });

            // 添加涨跌幅列
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChangePercent",
                HeaderText = "涨跌幅",
                DataPropertyName = "ChangePercent",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N4" }
            });

            // 添加是否小窗显示列
            dgvStocks.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "ShowInMiniDisplay",
                HeaderText = "小窗显示",
                DataPropertyName = "ShowInMiniDisplay",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // 添加删除按钮列（移到最后一列）
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "操作",
                Text = "删除",
                UseColumnTextForButtonValue = true,
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dgvStocks.Columns.Add(deleteColumn);

            // 添加CellClick事件处理程序，用于处理删除按钮点击
            dgvStocks.CellClick += dgvStocks_CellClick;

            // 添加CellValueChanged事件处理程序，用于处理小窗显示复选框变更
            dgvStocks.CellValueChanged += dgvStocks_CellValueChanged;

            // 添加CellFormatting事件处理程序，用于设置涨跌幅列的格式和颜色
            dgvStocks.CellFormatting += dgvStocks_CellFormatting;

            // 添加CurrentCellDirtyStateChanged事件处理程序，确保复选框立即生效
            dgvStocks.CurrentCellDirtyStateChanged += dgvStocks_CurrentCellDirtyStateChanged;

            // 初始化数据源
            _stockList = new List<Stock>();
            // 初始化BindingSource
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _stockList;
            dgvStocks.DataSource = _bindingSource;
        }

        #endregion

        #region 配置管理

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                // 确保目录存在
                string configDir = Path.GetDirectoryName(_configFilePath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                if (File.Exists(_configFilePath))
                {
                    string jsonString = File.ReadAllText(_configFilePath);
                    _config = JsonSerializer.Deserialize<AppConfig>(jsonString);

                    // 确保配置对象和集合不为空
                    if (_config == null)
                    {
                        _config = new AppConfig();
                    }
                    if (_config.StockCodes == null)
                    {
                        _config.StockCodes = new List<string>();
                    }
                }
                else
                {
                    _config = new AppConfig();
                    SaveConfig();
                }

                // 更新UI
                chkEnableScrolling.Checked = _config.EnableScrolling;
                chkEnableMiniDisplay.Checked = _config.EnableMiniDisplay;
                chkMiniDisplayTopMost.Checked = _config.MiniDisplayTopMost;

                // 根据配置控制滚动定时器
                UpdateScrollTimerStatus();

                // 根据配置控制小窗口显示
                UpdateMiniDisplayStatus();
            }
            catch (Exception)
            {
                // 加载配置失败，使用默认配置
                _config = new AppConfig();
                SaveConfig();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns>保存是否成功</returns>
        private bool SaveConfig()
        {
            try
            {
                // 确保目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));

                string jsonString = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFilePath, jsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region API客户端管理

        /// <summary>
        /// 更新API客户端
        /// </summary>
        private void UpdateApiClient()
        {
            if (_apiClient != null)
            {
                _apiClient.Dispose();
            }

            // 新浪API不需要token，直接初始化
            _apiClient = new ApiClient(string.Empty);
            // 订阅API日志事件
            _apiClient.ApiLog += (sender, e) => UpdateApiLog(e.Message);
        }

        #endregion

        #region 数据更新

        /// <summary>
        /// 更新股票数据
        /// </summary>
        private async Task UpdateStockDataAsync()
        {
            if (_apiClient != null && _config.StockCodes.Count > 0)
            {
                try
                {
                    // 显示加载状态
                    UpdateStatusLabel("正在更新数据...");

                    // 在后台线程获取数据
                    List<Stock> stockData = await _apiClient.GetStockDataAsync(_config.StockCodes);

                    // 在UI线程更新数据
                    this.Invoke((MethodInvoker)delegate
                    {
                        // 使用更高效的数据更新方式，避免频繁重新绑定数据源
                        UpdateDataGridViewData(stockData);

                        // 更新滚动文本
                        UpdateScrollText();

                        // 显示更新成功状态
                        UpdateStatusLabel($"数据更新成功，共 {_stockList.Count} 只股票");
                    });
                }
                catch (Exception ex)
                {
                    // 在UI线程更新状态
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateStatusLabel($"数据更新失败：{ex.Message}");
                    });
                }
            }
            else
            {
                string message = string.Empty;
                if (_apiClient == null)
                    message = "请先配置API Token";
                else if (_config.StockCodes.Count == 0)
                    message = "请先添加股票代码";
                else
                    message = "请先配置API Token和添加股票代码";

                UpdateStatusLabel(message);
            }
        }

        /// <summary>
        /// 高效更新DataGridView数据
        /// </summary>
        /// <param name="newData">新的股票数据列表</param>
        private void UpdateDataGridViewData(List<Stock> newData)
        {
            // 暂停DataGridView绘制，减少闪烁
            dgvStocks.SuspendLayout();

            try
            {
                // 更新内部数据列表
                _stockList.Clear();
                if (newData != null)
                {
                    // 添加所有新数据
                    _stockList.AddRange(newData);
                }

                // 使用BindingSource来更新数据，提高性能
                var bindingSource = new BindingSource();
                bindingSource.DataSource = _stockList;
                dgvStocks.DataSource = bindingSource;

                // 更新小窗口数据
                if (_miniDisplayForm != null)
                {
                    _miniDisplayForm.SetStockData(_stockList);
                }
            }
            finally
            {
                // 恢复DataGridView绘制
                dgvStocks.ResumeLayout();
            }
        }

        /// <summary>
        /// 更新状态栏标签
        /// </summary>
        /// <param name="message">状态消息</param>
        private void UpdateStatusLabel(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { tsStatusLabel.Text = message; });
            }
            else
            {
                tsStatusLabel.Text = message;
            }
        }

        #endregion

        #region 滚动显示管理

        /// <summary>
        /// 更新滚动文本
        /// </summary>
        private void UpdateScrollText()
        {
            if (_stockList.Count > 0)
            {
                // 生成系统托盘滚动文本
                _scrollText = string.Join(" | ", _stockList.Select(s => $"{s.Name}: {s.CurrentPrice:N2} ({s.ChangePercent:N2}%)"));
                _scrollText = $" {_scrollText} "; // 添加前后空格，实现无缝滚动
            }
            else
            {
                _scrollText = " 暂无股票数据 ";
            }

            _scrollPosition = 0;
        }

        /// <summary>
        /// 更新滚动定时器状态
        /// </summary>
        private void UpdateScrollTimerStatus()
        {
            if (_config.EnableScrolling)
            {
                timerScroll.Start();
            }
            else
            {
                timerScroll.Stop();
                // 关闭滚动时，清空系统托盘文本
                notifyIcon1.Text = "StockTrade";
            }
        }

        #endregion

        #region 事件处理程序

        /// <summary>
        /// 保存Token按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSaveToken_Click(object sender, EventArgs e)
        {
            // 新浪API不需要API Token，此功能已废弃
            MessageBox.Show("新浪API不需要API Token", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 添加股票按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAddStock_Click(object sender, EventArgs e)
        {
            string input = txtStockCode.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("请输入股票代码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 解析输入的股票代码，支持逗号分隔
            string[] codes = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            int addedCount = 0;

            foreach (string code in codes)
            {
                string trimmedCode = code.Trim();
                if (!string.IsNullOrEmpty(trimmedCode) && !_config.StockCodes.Contains(trimmedCode))
                {
                    _config.StockCodes.Add(trimmedCode);
                    addedCount++;
                }
            }

            if (addedCount == 0)
            {
                MessageBox.Show("没有添加新的股票代码，可能代码已存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 保存配置
            bool saved = SaveConfig();

            if (saved)
            {
                // 清空输入框
                txtStockCode.Text = string.Empty;

                // 更新数据
                await UpdateStockDataAsync();

                // 显示添加结果和数据查询结果
                string resultMessage = $"成功添加 {addedCount} 只股票代码";

                if (_stockList.Count > 0)
                {
                    resultMessage += $"\n\n成功查询到 {_stockList.Count} 只股票数据";

                    // 显示部分成功和部分失败的情况
                    if (_stockList.Count < _config.StockCodes.Count)
                    {
                        resultMessage += $"\n\n注意：有 {_config.StockCodes.Count - _stockList.Count} 只股票数据查询失败";
                    }

                    MessageBox.Show(resultMessage, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    resultMessage += "\n\n所有股票数据查询失败，请检查API Token和网络连接";
                    MessageBox.Show(resultMessage, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("股票代码保存失败，请检查权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await UpdateStockDataAsync();
        }

        /// <summary>
        /// 清空股票数据按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            // 清空所有股票代码
            _config.StockCodes.Clear();
            SaveConfig();
            // 更新股票数据
            _stockList.Clear();
            _bindingSource.ResetBindings(false);
            UpdateStatusLabel("已清空所有股票数据");
            // 更新小窗口数据
            if (_miniDisplayForm != null)
            {
                _miniDisplayForm.SetStockData(_stockList);
            }
            // 清空滚动文本
            _scrollText = string.Empty;
        }

        /// <summary>
        /// 判断当前时间是否在交易时间内
        /// </summary>
        /// <returns>是否在交易时间内</returns>
        private bool IsInTradingTime()
        {
            // 新浪API不限制访问时间，始终返回true
            return true;
        }

        /// <summary>
        /// 定时更新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void timerUpdate_Tick(object sender, EventArgs e)
        {
            await UpdateStockDataAsync();
        }

        /// <summary>
        /// 开关按钮状态变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEnableScrolling_CheckedChanged(object sender, EventArgs e)
        {
            // 只有当值真正改变时才执行操作，避免初始化时触发
            if (_config.EnableScrolling != chkEnableScrolling.Checked)
            {
                _config.EnableScrolling = chkEnableScrolling.Checked;
                bool saved = SaveConfig();

                if (saved)
                {
                    UpdateScrollTimerStatus();

                    string message = _config.EnableScrolling
                        ? "已开启任务栏滚动显示"
                        : "已关闭任务栏滚动显示";
                    MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 恢复之前的状态
                    chkEnableScrolling.Checked = !_config.EnableScrolling;
                    _config.EnableScrolling = chkEnableScrolling.Checked;
                    MessageBox.Show("配置保存失败，请检查权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 小窗口显示开关状态变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEnableMiniDisplay_CheckedChanged(object sender, EventArgs e)
        {
            // 只有当值真正改变时才执行操作，避免初始化时触发
            if (_config.EnableMiniDisplay != chkEnableMiniDisplay.Checked)
            {
                bool originalValue = _config.EnableMiniDisplay;
                _config.EnableMiniDisplay = chkEnableMiniDisplay.Checked;
                bool saved = SaveConfig();

                if (saved)
                {
                    UpdateMiniDisplayStatus();
                }
                else
                {
                    // 恢复之前的状态
                    _config.EnableMiniDisplay = originalValue;
                    chkEnableMiniDisplay.Checked = originalValue;
                    MessageBox.Show("配置保存失败，请检查权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 小窗口置顶开关状态变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkMiniDisplayTopMost_CheckedChanged(object sender, EventArgs e)
        {
            // 只有当值真正改变时才执行操作，避免初始化时触发
            if (_config.MiniDisplayTopMost != chkMiniDisplayTopMost.Checked)
            {
                _config.MiniDisplayTopMost = chkMiniDisplayTopMost.Checked;
                bool saved = SaveConfig();

                if (saved)
                {
                    UpdateMiniDisplayStatus();
                }
                else
                {
                    // 恢复之前的状态
                    chkMiniDisplayTopMost.Checked = !_config.MiniDisplayTopMost;
                    _config.MiniDisplayTopMost = chkMiniDisplayTopMost.Checked;
                    MessageBox.Show("配置保存失败，请检查权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 小窗配置按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMiniConfig_Click(object sender, EventArgs e)
        {
            // 创建并显示配置窗口
            using (MiniDisplayConfigForm configForm = new MiniDisplayConfigForm(_config))
            {
                // 订阅配置变更事件
                configForm.ConfigChanged += (s, args) =>
                {
                    // 保存配置
                    SaveConfig();
                    // 更新小窗口外观
                    _miniDisplayForm.ApplyConfig(_config);
                };

                // 显示对话框
                configForm.ShowDialog();
            }
        }

        /// <summary>
        /// 快捷键配置按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHotkeyConfig_Click(object sender, EventArgs e)
        {
            // 创建并显示快捷键配置窗口
            using (GlobalHotkeyConfigForm configForm = new GlobalHotkeyConfigForm(_config.GlobalHotkey))
            {
                // 订阅热键变更事件
                configForm.HotkeyChanged += (s, hotkey) =>
                {
                    // 更新配置
                    _config.GlobalHotkey = hotkey;
                    // 保存配置
                    SaveConfig();
                    // 重新注册热键
                    RegisterGlobalHotkey();
                };

                // 显示对话框
                configForm.ShowDialog();
            }
        }

        /// <summary>
        /// 滚动系统托盘文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerScroll_Tick(object sender, EventArgs e)
        {
            // 保持原有系统托盘滚动功能
            if (!string.IsNullOrEmpty(_scrollText))
            {
                // 系统托盘文本长度有限制，通常为64个字符
                int maxLength = 64;
                string displayText;

                // 计算当前要显示的文本
                if (_scrollText.Length <= maxLength)
                {
                    displayText = _scrollText;
                }
                else
                {
                    // 实现滚动效果
                    int startIndex = _scrollPosition % _scrollText.Length;
                    if (startIndex + maxLength <= _scrollText.Length)
                    {
                        displayText = _scrollText.Substring(startIndex, maxLength);
                    }
                    else
                    {
                        // 处理文本环绕的情况
                        displayText = _scrollText.Substring(startIndex) + _scrollText.Substring(0, maxLength - (_scrollText.Length - startIndex));
                    }

                    _scrollPosition++;
                    if (_scrollPosition >= _scrollText.Length)
                    {
                        _scrollPosition = 0;
                    }
                }

                // 更新系统托盘文本
                notifyIcon1.Text = displayText;
            }
        }

        /// <summary>
        /// DataGridView单元格点击事件，用于处理删除按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvStocks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 确保点击的是删除按钮列
            if (e.ColumnIndex == dgvStocks.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                // 获取要删除的股票
                Stock stock = (Stock)dgvStocks.Rows[e.RowIndex].DataBoundItem;
                string stockName = stock.Name;
                string stockDisplayCode = stock.Code;

                // 查找要删除的股票代码（处理API返回的代码与配置中不一致的情况）
                string stockCodeToRemove = null;
                foreach (string code in _config.StockCodes)
                {
                    // 匹配完整代码或前缀匹配（例如：配置中的"AAPL"匹配API返回的"AAPL.US"）
                    if (stockDisplayCode == code || stockDisplayCode.StartsWith(code + "."))
                    {
                        stockCodeToRemove = code;
                        break;
                    }
                }

                // 如果找到匹配的股票代码
                if (stockCodeToRemove != null)
                {
                    // 显示确认对话框
                    DialogResult result = MessageBox.Show(
                        $"确定要删除股票 {stockName} ({stockDisplayCode}) 吗？",
                        "删除确认",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.Yes)
                    {
                        // 从配置中移除股票代码
                        _config.StockCodes.Remove(stockCodeToRemove);

                        // 保存配置
                        bool saved = SaveConfig();

                        if (saved)
                        {
                            // 立即清空DataGridView并更新UI
                            this.Invoke((MethodInvoker)delegate
                            {
                                // 先清空DataGridView
                                UpdateDataGridViewData(new List<Stock>());

                                // 如果还有股票代码，更新数据
                                if (_config.StockCodes.Count > 0)
                                {
                                    // 在UI线程更新状态，然后在后台线程更新数据
                                    UpdateStatusLabel("正在更新数据...");
                                    Task.Run(async () => await UpdateStockDataAsync());
                                }
                                else
                                {
                                    // 更新滚动文本和状态
                                    UpdateScrollText();
                                    UpdateStatusLabel("请先添加股票代码");
                                }
                            });

                            // 显示成功消息
                            MessageBox.Show($"股票 {stockName} ({stockDisplayCode}) 删除成功", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // 显示失败消息
                            MessageBox.Show("股票删除失败，请检查权限", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    // 显示找不到股票代码的消息
                    MessageBox.Show("找不到对应的股票代码，删除失败", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 当前单元格状态变更事件，确保复选框立即生效
        /// </summary>
        private void dgvStocks_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvStocks.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvStocks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// 单元格值变更事件，处理小窗显示复选框变更
        /// </summary>
        private void dgvStocks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 确保变更的是小窗显示列
            if (e.ColumnIndex == dgvStocks.Columns["ShowInMiniDisplay"].Index && e.RowIndex >= 0)
            {
                // 更新小窗口显示
                UpdateMiniDisplay();
            }
        }

        /// <summary>
        /// 更新小窗口显示的数据
        /// </summary>
        private void UpdateMiniDisplay()
        {
            if (_miniDisplayForm != null)
            {
                _miniDisplayForm.SetStockData(_stockList);
            }
        }

        /// <summary>
        /// 退出程序按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            // 显示确认对话框
            DialogResult result = MessageBox.Show(
                "确定要退出StockTrade吗？",
                "确认退出",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                // 停止所有定时器
                timerUpdate.Stop();
                timerScroll.Stop();

                // 释放API客户端资源
                _apiClient?.Dispose();

                // 退出应用程序
                Application.Exit();
            }
        }

        /// <summary>
        /// 系统托盘图标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ShowHideForm();
        }

        /// <summary>
        /// 显示窗口菜单项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideForm();
        }

        /// <summary>
        /// 退出菜单项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnExit_Click(sender, e);
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 注销热键
            UnregisterHotKey(this.Handle, HOTKEY_ID);

            // 直接退出程序，不隐藏窗口
            // 停止所有定时器
            timerUpdate.Stop();
            timerScroll.Stop();

            // 释放API客户端资源
            _apiClient?.Dispose();
        }

        /// <summary>
        /// 解析热键字符串
        /// </summary>
        /// <param name="hotkeyStr">热键字符串，如 "Ctrl+Alt+Q"</param>
        /// <param name="modifiers">输出修饰符</param>
        /// <param name="vk">输出虚拟键码</param>
        private void ParseHotkey(string hotkeyStr, out int modifiers, out int vk)
        {
            modifiers = 0;
            vk = (int)Keys.Q; // 默认Q键

            if (string.IsNullOrEmpty(hotkeyStr))
                return;

            string[] parts = hotkeyStr.Split('+');

            for (int i = 0; i < parts.Length - 1; i++)
            {
                switch (parts[i].ToLower())
                {
                    case "alt":
                        modifiers |= MOD_ALT;
                        break;
                    case "ctrl":
                        modifiers |= MOD_CONTROL;
                        break;
                    case "shift":
                        modifiers |= MOD_SHIFT;
                        break;
                }
            }

            if (parts.Length > 0)
            {
                Keys key;
                if (Enum.TryParse(parts[^1], out key))
                {
                    vk = (int)key;
                }
            }
        }

        /// <summary>
        /// 注册全局热键
        /// </summary>
        private void RegisterGlobalHotkey()
        {
            // 先注销现有热键
            UnregisterHotKey(this.Handle, HOTKEY_ID);

            // 解析热键
            int modifiers, vk;
            ParseHotkey(_config.GlobalHotkey, out modifiers, out vk);

            // 注册新热键
            RegisterHotKey(this.Handle, HOTKEY_ID, modifiers, vk);
        }

        /// <summary>
        /// 窗口加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 注册全局热键
            RegisterGlobalHotkey();

            // 初始隐藏窗口，只显示在系统托盘
            this.Hide();

            // 加载保存的刷新间隔
            nudUpdateInterval.Value = _config.UpdateInterval;
        }

        /// <summary>
        /// 更新小窗口显示状态
        /// </summary>
        private void UpdateMiniDisplayStatus()
        {
            if (_miniDisplayForm != null)
            {
                // 根据配置显示或隐藏小窗口
                if (_config.EnableMiniDisplay)
                {
                    _miniDisplayForm.Show();
                    // 使用新的SetTopMostStyle方法设置最顶层样式
                    _miniDisplayForm.SetTopMostStyle(_config.MiniDisplayTopMost);
                }
                else
                {
                    _miniDisplayForm.Hide();
                }
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 显示/隐藏窗口
        /// </summary>
        private void ShowHideForm()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        /// <summary>
        /// 更新API日志
        /// </summary>
        /// <param name="message">日志消息</param>
        private void UpdateApiLog(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateApiLog(message); });
                return;
            }

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            rtbApiLog.AppendText(logEntry + Environment.NewLine);

            // 自动滚动到最底部
            rtbApiLog.SelectionStart = rtbApiLog.TextLength;
            rtbApiLog.ScrollToCaret();
        }

        /// <summary>
        /// 重写处理快捷键
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 处理Alt+Q快捷键
            if (keyData == (Keys.Alt | Keys.Q))
            {
                ShowHideForm();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 应用刷新间隔按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyInterval_Click(object sender, EventArgs e)
        {
            int newInterval = (int)nudUpdateInterval.Value;
            _config.UpdateInterval = newInterval;

            // 保存配置
            bool saved = SaveConfig();

            if (saved)
            {
                // 更新定时器间隔
                timerUpdate.Interval = newInterval * 1000;

                // 显示成功消息
                UpdateStatusLabel($"刷新间隔已设置为 {newInterval} 秒");
            }
            else
            {
                UpdateStatusLabel("刷新间隔保存失败");
            }
        }

        /// <summary>
        /// 单元格格式化事件，用于设置涨跌幅列的格式和颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvStocks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 检查是否是涨跌幅列
            if (e.ColumnIndex == dgvStocks.Columns["ChangePercent"].Index && e.RowIndex >= 0)
            {
                // 确保单元格值是数值类型
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal changePercent))
                {
                    // 格式化涨跌幅为百分比，保留2位小数
                    e.Value = $"{changePercent:F2}%";

                    // 设置颜色：大于0红色，小于0绿色，等于0黑色
                    if (changePercent > 0)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    else if (changePercent < 0)
                    {
                        e.CellStyle.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Black;
                    }

                    e.FormattingApplied = true;
                }
            }
        }

        /// <summary>
        /// 窗口大小改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            // 当窗口最小化时，隐藏窗口
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// 处理Windows消息，包括全局热键
        /// </summary>
        /// <param name="m">消息</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 处理热键消息
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    ShowHideForm();
                }
            }
        }

        #endregion
    }
}