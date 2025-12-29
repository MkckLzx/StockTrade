namespace StockTrade
{
    /// <summary>
    /// 股票信息类，用于存储股票的基本信息和实时数据
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// 股票代码（如：600000）
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// 股票名称（如：浦发银行）
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 开盘价格
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// 涨跌金额（当前价 - 昨收价）
        /// </summary>
        public decimal ChangeAmount { get; set; }

        /// <summary>
        /// 涨跌幅（百分比，涨跌金额 / 昨收价 * 100）
        /// </summary>
        public decimal ChangePercent { get; set; }

        /// <summary>
        /// 是否在小窗中显示
        /// </summary>
        public bool ShowInMiniDisplay { get; set; } = false;
    }

    /// <summary>
    /// 小窗口显示字段枚举，用于控制小窗口中显示哪些股票字段
    /// </summary>
    [Flags]
    public enum MiniDisplayFields
    {
        /// <summary>
        /// 不显示任何字段
        /// </summary>
        None = 0,
        /// <summary>
        /// 显示股票名称
        /// </summary>
        Name = 1,
        /// <summary>
        /// 显示股票代码
        /// </summary>
        Code = 2,
        /// <summary>
        /// 显示当前价格
        /// </summary>
        CurrentPrice = 4,
        /// <summary>
        /// 显示开盘价格
        /// </summary>
        OpenPrice = 8,
        /// <summary>
        /// 显示涨跌金额
        /// </summary>
        ChangeAmount = 16,
        /// <summary>
        /// 显示涨跌幅
        /// </summary>
        ChangePercent = 32
    }

    /// <summary>
    /// 应用程序配置类，用于存储应用程序的所有配置项
    /// </summary>
    public class AppConfig
    {
        #region 基本配置
        
        /// <summary>
        /// 自选股代码列表
        /// </summary>
        public List<string> StockCodes { get; set; }

        /// <summary>
        /// 定时更新间隔（秒）
        /// </summary>
        public int UpdateInterval { get; set; }

        #endregion

        #region 任务栏滚动配置
        
        /// <summary>
        /// 是否开启任务栏滚动显示
        /// </summary>
        public bool EnableScrolling { get; set; }

        #endregion

        #region 小窗口显示配置
        
        /// <summary>
        /// 是否开启小窗口显示
        /// </summary>
        public bool EnableMiniDisplay { get; set; }

        /// <summary>
        /// 小窗口是否置顶显示
        /// </summary>
        public bool MiniDisplayTopMost { get; set; }

        /// <summary>
        /// 小窗口宽度
        /// </summary>
        public int MiniDisplayWidth { get; set; }

        /// <summary>
        /// 小窗口高度
        /// </summary>
        public int MiniDisplayHeight { get; set; }

        /// <summary>
        /// 小窗口显示字段（位掩码）
        /// </summary>
        public MiniDisplayFields MiniDisplayShowFields { get; set; }

        /// <summary>
        /// 小窗口滚动时间间隔（毫秒）
        /// </summary>
        public int MiniDisplayScrollInterval { get; set; }

        #endregion

        #region 小窗口外观配置
        
        /// <summary>
        /// 小窗口背景颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayBgColor { get; set; }

        /// <summary>
        /// 小窗口背景透明度（0-100）
        /// </summary>
        public int MiniDisplayBgOpacity { get; set; }

        /// <summary>
        /// 小窗口默认字体颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayFontColor { get; set; }

        /// <summary>
        /// 小窗口字体透明度（0-100）
        /// </summary>
        public int MiniDisplayFontOpacity { get; set; }

        /// <summary>
        /// 小窗口字体名称
        /// </summary>
        public string MiniDisplayFontFamily { get; set; }

        /// <summary>
        /// 小窗口字体大小
        /// </summary>
        public float MiniDisplayFontSize { get; set; }

        #endregion

        #region 小窗口字段颜色配置
        
        /// <summary>
        /// 名称字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayNameColor { get; set; }
        
        /// <summary>
        /// 代码字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayCodeColor { get; set; }
        
        /// <summary>
        /// 现价字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayCurrentPriceColor { get; set; }
        
        /// <summary>
        /// 开盘价字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayOpenPriceColor { get; set; }
        
        /// <summary>
        /// 涨跌额字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayChangeAmountColor { get; set; }
        
        /// <summary>
        /// 涨跌幅字段颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayChangePercentColor { get; set; }

        #endregion

        #region 全局快捷键配置
        
        /// <summary>
        /// 全局快捷键（用于隐藏/显示程序，格式："Ctrl+Alt+Q"）
        /// </summary>
        public string GlobalHotkey { get; set; }

        #endregion

        /// <summary>
        /// 构造函数，初始化所有配置项的默认值
        /// </summary>
        public AppConfig()
        {
            // 基本配置
            StockCodes = new List<string>();
            UpdateInterval = 30; // 默认30秒更新一次
            
            // 任务栏滚动配置
            EnableScrolling = false; // 默认关闭任务栏滚动显示
            
            // 小窗口显示配置
            EnableMiniDisplay = false; // 默认关闭小窗口显示
            MiniDisplayTopMost = false; // 默认小窗口不置顶
            MiniDisplayWidth = 200; // 默认宽度
            MiniDisplayHeight = 60; // 默认高度
            MiniDisplayShowFields = MiniDisplayFields.Name | MiniDisplayFields.ChangePercent; // 默认显示名称和涨跌幅
            MiniDisplayScrollInterval = 3000; // 默认滚动时间间隔3秒
            
            // 小窗口外观配置
            MiniDisplayBgColor = "0,0,0"; // 默认黑色背景
            MiniDisplayBgOpacity = 90; // 默认背景透明度90%
            MiniDisplayFontColor = "255,255,255"; // 默认白色字体
            MiniDisplayFontOpacity = 100; // 默认字体透明度100%
            MiniDisplayFontFamily = "Microsoft YaHei"; // 默认字体
            MiniDisplayFontSize = 10f; // 默认字体大小
            
            // 小窗口字段颜色配置
            MiniDisplayNameColor = "255,255,255"; // 名称默认白色
            MiniDisplayCodeColor = "255,255,255"; // 代码默认白色
            MiniDisplayCurrentPriceColor = "255,255,255"; // 现价默认白色
            MiniDisplayOpenPriceColor = "255,255,255"; // 开盘价默认白色
            MiniDisplayChangeAmountColor = "255,255,255"; // 涨跌额默认白色
            MiniDisplayChangePercentColor = "255,255,255"; // 涨跌幅默认白色
            
            // 全局快捷键配置
            GlobalHotkey = "Alt+Q"; // 默认全局快捷键Alt+Q
        }
    }
}