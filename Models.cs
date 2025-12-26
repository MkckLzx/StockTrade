namespace StockTrade
{
    /// <summary>
    /// 股票信息类
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// 股票名称
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
        /// 涨跌金额
        /// </summary>
        public decimal ChangeAmount { get; set; }

        /// <summary>
        /// 涨跌幅（百分比）
        /// </summary>
        public decimal ChangePercent { get; set; }

        /// <summary>
        /// 是否在小窗中显示
        /// </summary>
        public bool ShowInMiniDisplay { get; set; } = true;
    }

    /// <summary>
    /// 小窗口显示字段枚举
    /// </summary>
    [Flags]
    public enum MiniDisplayFields
    {
        None = 0,
        Name = 1,
        Code = 2,
        CurrentPrice = 4,
        OpenPrice = 8,
        ChangeAmount = 16,
        ChangePercent = 32
    }

    /// <summary>
    /// 应用程序配置类
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// iTick API Token
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// 自选股代码列表
        /// </summary>
        public List<string> StockCodes { get; set; }

        /// <summary>
        /// 定时更新间隔（秒）
        /// </summary>
        public int UpdateInterval { get; set; }

        /// <summary>
        /// 是否开启任务栏滚动显示
        /// </summary>
        public bool EnableScrolling { get; set; }

        /// <summary>
        /// 开盘时间（格式：HH:mm）
        /// </summary>
        public string OpenTime { get; set; }

        /// <summary>
        /// 休市时间（格式：HH:mm）
        /// </summary>
        public string CloseTime { get; set; }

        /// <summary>
        /// 是否开启小窗口显示
        /// </summary>
        public bool EnableMiniDisplay { get; set; }

        /// <summary>
        /// 小窗口是否置顶显示
        /// </summary>
        public bool MiniDisplayTopMost { get; set; }

        /// <summary>
        /// 小窗口背景颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayBgColor { get; set; }

        /// <summary>
        /// 小窗口默认字体颜色（RGB值，格式："R,G,B"）
        /// </summary>
        public string MiniDisplayFontColor { get; set; }
        
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
        /// 小窗口背景透明度（0-100）
        /// </summary>
        public int MiniDisplayBgOpacity { get; set; }
        
        /// <summary>
        /// 小窗口字体透明度（0-100）
        /// </summary>
        public int MiniDisplayFontOpacity { get; set; }
        
        /// <summary>
        /// 全局快捷键（用于隐藏/显示程序）
        /// </summary>
        public string GlobalHotkey { get; set; }

        /// <summary>
        /// 小窗口字体名称
        /// </summary>
        public string MiniDisplayFontFamily { get; set; }

        /// <summary>
        /// 小窗口字体大小
        /// </summary>
        public float MiniDisplayFontSize { get; set; }

        /// <summary>
        /// 小窗口滚动时间间隔（毫秒）
        /// </summary>
        public int MiniDisplayScrollInterval { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AppConfig()
        {
            ApiToken = string.Empty;
            StockCodes = new List<string>();
            UpdateInterval = 30; // 默认30秒更新一次
            EnableScrolling = false; // 默认关闭任务栏滚动显示
            OpenTime = "09:30"; // 默认开盘时间9:30
            CloseTime = "15:00"; // 默认休市时间15:00
            EnableMiniDisplay = false; // 默认关闭小窗口显示
            MiniDisplayTopMost = false; // 默认小窗口不置顶
            
            // 小窗口默认配置
            MiniDisplayBgColor = "0,0,0"; // 默认黑色背景
            MiniDisplayFontColor = "255,255,255"; // 默认白色字体
            MiniDisplayNameColor = "255,255,255"; // 名称默认白色
            MiniDisplayCodeColor = "255,255,255"; // 代码默认白色
            MiniDisplayCurrentPriceColor = "255,255,255"; // 现价默认白色
            MiniDisplayOpenPriceColor = "255,255,255"; // 开盘价默认白色
            MiniDisplayChangeAmountColor = "255,255,255"; // 涨跌额默认白色
            MiniDisplayChangePercentColor = "255,255,255"; // 涨跌幅默认白色
            MiniDisplayWidth = 200; // 默认宽度
            MiniDisplayHeight = 60; // 默认高度
            MiniDisplayShowFields = MiniDisplayFields.Name | MiniDisplayFields.ChangePercent; // 默认显示名称和涨跌幅
            MiniDisplayBgOpacity = 90; // 默认背景透明度90%
            MiniDisplayFontOpacity = 100; // 默认字体透明度100%
            GlobalHotkey = "Alt+Q"; // 默认全局快捷键Alt+Q
            MiniDisplayFontFamily = "Microsoft YaHei"; // 默认字体
            MiniDisplayFontSize = 10f; // 默认字体大小
            MiniDisplayScrollInterval = 3000; // 默认滚动时间间隔3秒
        }
    }
}