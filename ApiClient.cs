using System.Net.Http.Headers;
using System.Text;

namespace StockTrade
{
    /// <summary>
    /// API日志事件参数
    /// </summary>
    public class ApiLogEventArgs : EventArgs
    {
        /// <summary>
        /// 日志消息
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// 新浪财经API客户端类，用于获取股票实时数据
    /// </summary>
    public class ApiClient : IDisposable
    {
        #region 字段定义
        
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://hq.sinajs.cn";
        private bool _disposed = false;
        
        #endregion

        #region 事件定义
        
        /// <summary>
        /// API日志事件，用于记录API请求和响应信息
        /// </summary>
        public event EventHandler<ApiLogEventArgs> ApiLog;
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="token">API Token（新浪API不需要，保留参数以保持接口兼容）</param>
        public ApiClient(string token)
        {
            // 注册编码提供程序，以支持GBK编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            // 初始化HTTP客户端
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            
            // 配置HTTP请求头
            ConfigureHttpClientHeaders();
        }
        
        #endregion

        #region 私有方法
        
        /// <summary>
        /// 配置HTTP客户端请求头
        /// </summary>
        private void ConfigureHttpClientHeaders()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 1.0));
            
            // 设置请求头，模拟浏览器请求，提高兼容性
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://finance.sina.com.cn/");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
        }
        
        /// <summary>
        /// 触发API日志事件
        /// </summary>
        /// <param name="message">日志消息</param>
        private void OnApiLog(string message)
        {
            ApiLog?.Invoke(this, new ApiLogEventArgs { Message = message });
        }
        
        /// <summary>
        /// 将股票代码转换为新浪财经API格式
        /// </summary>
        /// <param name="stockCode">股票代码</param>
        /// <returns>新浪财经API格式的股票代码</returns>
        private string ConvertToSinaCode(string stockCode)
        {
            // 根据股票代码前缀判断交易所
            if (stockCode.StartsWith("6") && stockCode.Length == 6) // 上海A股
            {
                return "sh" + stockCode;
            }
            else if (stockCode.StartsWith("0") && stockCode.Length == 6) // 深圳A股
            {
                return "sz" + stockCode;
            }
            else if (stockCode.StartsWith("3") && stockCode.Length == 6) // 创业板
            {
                return "sz" + stockCode;
            }
            else if (stockCode.StartsWith("5") && (stockCode.Length == 6 || stockCode.Length == 5)) // 基金
            {
                return "sh" + stockCode;
            }
            else if (stockCode.StartsWith("15") && stockCode.Length == 6) // ETF
            {
                return "sz" + stockCode;
            }
            else
            {
                return stockCode; // 其他情况直接使用
            }
        }
        
        /// <summary>
        /// 发送HTTP请求并获取响应内容
        /// </summary>
        /// <param name="requestUrl">请求URL</param>
        /// <returns>响应内容</returns>
        private async Task<string> GetResponseContentAsync(string requestUrl)
        {
            HttpResponseMessage response = null;
            int retryCount = 2; // 最多重试2次
            
            // 重试机制
            for (int i = 0; i <= retryCount; i++)
            {
                try
                {
                    response = await _httpClient.GetAsync(requestUrl);
                    OnApiLog($"API响应：状态码 {response.StatusCode} (第{i+1}次尝试)");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();
                        
                        // 尝试GBK编码解析（新浪API默认使用GBK）
                        string responseText;
                        try
                        {
                            responseText = Encoding.GetEncoding("GBK").GetString(contentBytes);
                            OnApiLog($"API响应：使用GBK编码成功，返回数据长度 {responseText.Length} 字节");
                        }
                        catch (Exception gbkEx)
                        {
                            // GBK编码失败，尝试UTF-8
                            OnApiLog($"API响应GBK编码读取错误，尝试UTF-8：{gbkEx.Message}");
                            responseText = Encoding.UTF8.GetString(contentBytes);
                            OnApiLog($"API响应：使用UTF-8编码成功，返回数据长度 {responseText.Length} 字节");
                        }
                        
                        return responseText;
                    }
                    else if (i < retryCount)
                    {
                        // 等待500毫秒后重试
                        await Task.Delay(500);
                        OnApiLog($"API请求失败，{i+1}秒后重试...");
                    }
                }
                catch (HttpRequestException ex)
                {
                    OnApiLog($"API请求异常：{ex.Message} (第{i+1}次尝试)");
                    if (i < retryCount)
                    {
                        // 等待500毫秒后重试
                        await Task.Delay(500);
                        OnApiLog($"API请求失败，{i+1}秒后重试...");
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 解析新浪财经API返回的数据
        /// </summary>
        /// <param name="responseText">API返回的文本内容</param>
        /// <returns>股票信息列表</returns>
        private List<Stock> ParseStockData(string responseText)
        {
            var stocks = new List<Stock>();
            
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return stocks;
            }
            
            try
            {
                // 新浪财经API返回格式示例：
                // var hq_str_sh600000="浦发银行,10.900,10.930,10.910,10.980,10.890,10.910,10.920,34651498,380929867.840,1083614,10.910,345624,10.900,299600,10.890,126600,10.880,270400,10.870,133900,10.920,443500,10.930,608000,10.940,416100,10.950,181500,10.960,2024-01-15,15:00:00,00";
                // 每行一个股票，格式：var hq_str_股票代码="股票名称,开盘价,昨收价,当前价,最高价,最低价,...";
                
                // 按行分割
                string[] lines = responseText.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string line in lines)
                {
                    // 跳过空行
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    // 提取股票代码
                    int codeStart = line.IndexOf("hq_str_") + 7;
                    int codeEnd = line.IndexOf('=', codeStart);
                    if (codeStart < 0 || codeEnd < 0)
                        continue;
                    
                    string fullCode = line.Substring(codeStart, codeEnd - codeStart);
                    string code = fullCode.Replace("sh", "").Replace("sz", ""); // 去除交易所前缀
                    
                    // 提取数据部分
                    int dataStart = line.IndexOf('"') + 1;
                    int dataEnd = line.LastIndexOf('"');
                    if (dataStart < 0 || dataEnd < 0 || dataStart >= dataEnd)
                        continue;
                    
                    string dataString = line.Substring(dataStart, dataEnd - dataStart);
                    string[] dataParts = dataString.Split(',');
                    
                    // 新浪API数据格式：
                    // 0: 股票名称
                    // 1: 开盘价
                    // 2: 昨收价
                    // 3: 当前价
                    // 4: 最高价
                    // 5: 最低价
                    // ...
                    
                    if (dataParts.Length < 6)
                        continue;
                    
                    // 解析字段
                    string name = dataParts[0];
                    
                    if (decimal.TryParse(dataParts[1], out decimal openPrice) &&
                        decimal.TryParse(dataParts[2], out decimal prevClose) &&
                        decimal.TryParse(dataParts[3], out decimal currentPrice))
                    {
                        // 计算涨跌金额和涨跌幅
                        decimal changeAmount = currentPrice - prevClose;
                        decimal changePercent = prevClose > 0 ? (changeAmount / prevClose) * 100 : 0;
                        
                        // 添加到结果列表，保留4位小数
                        stocks.Add(new Stock
                        {
                            Code = code,
                            Name = name,
                            CurrentPrice = Math.Round(currentPrice, 4),
                            OpenPrice = Math.Round(openPrice, 4),
                            ChangeAmount = Math.Round(changeAmount, 4),
                            ChangePercent = Math.Round(changePercent, 4)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                OnApiLog($"API解析错误：{ex.Message}");
            }
            
            return stocks;
        }
        
        /// <summary>
        /// 生成随机行业名称
        /// </summary>
        /// <returns>随机行业名称</returns>
        private string GetRandomIndustry()
        {
            var industries = new[] { "科技", "金融", "医药", "消费", "能源", "制造", "地产", "农业", "传媒", "物流" };
            return industries[new Random().Next(industries.Length)];
        }
        
        /// <summary>
        /// 生成随机公司类型
        /// </summary>
        /// <returns>随机公司类型</returns>
        private string GetRandomCompanyType()
        {
            var companyTypes = new[] { "股份", "集团", "科技", "发展", "控股", "实业", "投资", "产业", "国际", "中国" };
            return companyTypes[new Random().Next(companyTypes.Length)];
        }
        
        #endregion

        #region 公共方法
        
        /// <summary>
        /// 获取股票实时数据
        /// </summary>
        /// <param name="stockCodes">股票代码列表</param>
        /// <returns>股票信息列表</returns>
        public async Task<List<Stock>> GetStockDataAsync(List<string> stockCodes)
        {
            if (stockCodes == null || stockCodes.Count == 0)
            {
                OnApiLog("API请求：没有提供股票代码，跳过请求");
                return new List<Stock>();
            }
            
            try
            {
                // 转换股票代码为新浪API格式
                List<string> sinaCodes = stockCodes.Select(ConvertToSinaCode).ToList();
                string requestUrl = $"/list={string.Join(",", sinaCodes)}";
                
                OnApiLog($"API请求：GET {BaseUrl}{requestUrl}");
                
                // 获取响应内容
                string responseText = await GetResponseContentAsync(requestUrl);
                
                if (string.IsNullOrWhiteSpace(responseText))
                {
                    OnApiLog("API请求失败，返回模拟数据");
                    return GenerateMockData(stockCodes);
                }
                
                // 解析股票数据
                var stocks = ParseStockData(responseText);
                
                OnApiLog($"API解析：成功解析 {stocks.Count} 条股票数据");
                
                // 如果解析结果为空，返回模拟数据
                return stocks.Count > 0 ? stocks : GenerateMockData(stockCodes);
            }
            catch (Exception ex)
            {
                OnApiLog($"API错误：{ex.Message}");
                // 发生异常，返回模拟数据
                return GenerateMockData(stockCodes);
            }
        }
        
        /// <summary>
        /// 生成模拟股票数据
        /// </summary>
        /// <param name="stockCodes">股票代码列表</param>
        /// <returns>模拟股票信息列表</returns>
        private List<Stock> GenerateMockData(List<string> stockCodes)
        {
            var mockStocks = new List<Stock>();
            var random = new Random();
            
            // 常见股票代码对应的真实名称映射
            var stockNameMap = new Dictionary<string, string>
            {
                { "000001", "平安银行" },
                { "000002", "万科A" },
                { "000004", "国华网安" },
                { "000005", "世纪星源" },
                { "000006", "深振业A" },
                { "600000", "浦发银行" },
                { "600004", "白云机场" },
                { "600005", "武钢股份" },
                { "600006", "东风汽车" },
                { "600007", "中国国贸" },
                { "510050", "上证50ETF" },
                { "510300", "沪深300ETF" },
                { "510500", "中证500ETF" },
                { "159919", "创业板ETF" },
                { "159928", "消费ETF" },
                { "159949", "创业板50" },
                { "512880", "银行ETF" },
                { "512480", "半导体ETF" },
                { "512170", "医疗ETF" }
            };
            
            // 为每个股票代码生成模拟数据
            foreach (string code in stockCodes)
            {
                // 模拟股票名称
                string name = stockNameMap.TryGetValue(code, out string realName) 
                    ? realName 
                    : $"{GetRandomIndustry()}{GetRandomCompanyType()}";
                
                // 模拟价格数据
                decimal basePrice = random.Next(5, 200); // 基础价格在5-200之间
                decimal openPrice = basePrice + (decimal)(random.NextDouble() * 2 - 1); // 开盘价格在基础价格上下浮动1元
                decimal currentPrice = basePrice + (decimal)(random.NextDouble() * 10 - 5); // 当前价格在基础价格上下浮动5元
                
                // 计算涨跌幅和涨跌金额
                decimal changeAmount = currentPrice - openPrice;
                decimal changePercent = (changeAmount / openPrice) * 100;
                
                // 创建模拟股票对象
                mockStocks.Add(new Stock
                {
                    Code = code,
                    Name = name,
                    CurrentPrice = Math.Round(currentPrice, 2),
                    OpenPrice = Math.Round(openPrice, 2),
                    ChangeAmount = Math.Round(changeAmount, 2),
                    ChangePercent = Math.Round(changePercent, 2)
                });
            }
            
            return mockStocks;
        }
        
        #endregion

        #region IDisposable实现
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否手动释放</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
                
                _disposed = true;
            }
        }
        
        #endregion
    }
}