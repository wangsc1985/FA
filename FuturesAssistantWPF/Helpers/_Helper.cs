using FuturesAssistantWPF.Types;
using FuturesAssistantWPF.FAException;
using FuturesAssistantWPF.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using FuturesAssistantWPF.Windows;
using System.Windows.Threading;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.Data;
using System.Windows;

namespace FuturesAssistantWPF.Helpers
{
    public static class _Helper
    {
        private delegate void FormControlInvoker();
        private const int TimeOut = 50000;
        private static Version httpVersion = HttpVersion.Version10;


        /// <summary>  
        /// 获取网络日期时间  
        /// </summary>  
        /// <returns></returns>  
        public static DateTime GetNetDateTime()
        {
            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;
            string datetime = string.Empty;
            try
            {
                request = WebRequest.Create("https://www.baidu.com");
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = (WebResponse)request.GetResponse();
                headerCollection = response.Headers;
                foreach (var h in headerCollection.AllKeys)
                { if (h == "Date") { datetime = headerCollection[h]; } }
                return Convert.ToDateTime(datetime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (request != null)
                { request.Abort(); }
                if (response != null)
                { response.Close(); }
                if (headerCollection != null)
                { headerCollection.Clear(); }
            }
        }

        /// <summary>
        /// 刷新指定控件
        /// </summary>
        /// <param name="value"></param>
        public static void _Refresh(this DispatcherObject value)
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f)
                {
                    ((DispatcherFrame)f).Continue = false;

                    return null;
                }
                    ), frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// 将double类型的数值转换为蜡烛线索引数值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double _ToStockPosition(this double value)
        {
            try
            {
                //7.5-8.5是第8根蜡烛线索引
                double fractional = value - (int)value;
                if (fractional >= 0.5)
                {
                    return (int)value + 1;
                }
                else
                {
                    return (int)value;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //
        // ：加密解密相关方法
        //
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="password">需要加密的密码</param>
        /// <returns>MD5加密后32位长度的字符串</returns>
        public static string _MD5Encrypt(this string password)
        {
            try
            {
                byte[] btPassword = Encoding.Default.GetBytes(password);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] btEncryptedPassword = md5.ComputeHash(btPassword);
                return BitConverter.ToString(btEncryptedPassword).Replace("-", "");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        public static string _RSAEcrypt(this string password)
        {
            try
            {
                CspParameters param = new CspParameters();
                param.KeyContainerName = "fuas";//密匙容器的名称，保持加密解密一致才能解密成功
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
                {
                    byte[] encryptdata = rsa.Encrypt(Encoding.UTF8.GetBytes(password), false);//将加密后的字节数据转换为新的加密字节数组
                    return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        public static string _RSADecrypt(this string ciphertext)
        {
            try
            {
                CspParameters param = new CspParameters();
                param.KeyContainerName = "fuas";
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
                {
                    byte[] decryptdata = rsa.Decrypt(Convert.FromBase64String(ciphertext), false);
                    return Encoding.UTF8.GetString(decryptdata);
                }
            }
            catch (Exception)
            {
                return "123456";
            }
        }

        //
        // ：设置保存程序参数相关方法
        //

        /// <summary>
        /// 向Parameter中设置或添加参数。
        /// 注：此方法已经自动设置UserId参数
        /// </summary>
        /// <param name="Name">要设置或添加的参数</param>
        /// <param name="value">要设置或添加的参数值</param>
        public static void SetParameter(string Name, string value)
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Models.Parameter)))
            {
                var val = statement.Parameters.FirstOrDefault(p => p.Name.Equals(Name) && p.UserId == _Session.LoginedUserId);
                if (val == null)
                {
                    FuturesAssistantWPF.Models.Parameter par = new FuturesAssistantWPF.Models.Parameter();
                    par.Id = Guid.NewGuid();
                    par.Name = Name;
                    par.UserId = _Session.LoginedUserId;
                    par.Value = value;
                    statement.AddParameter(par);
                }
                else
                {
                    val.Value = value;
                    statement.EditParameter(val);
                }
                statement.SaveChanged();
            }
        }

        /// <summary>
        /// 向Parameter中设置或添加参数。
        /// </summary>
        /// <param name="Name">要设置或添加的参数</param>
        /// <param name="value">要设置或添加的参数值</param>
        /// <param name="userId">此参数所属User</param>
        public static void SetParameter(string Name, string value, User user)
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Models.Parameter)))
            {
                var val = statement.Parameters.FirstOrDefault(p => p.Name.Equals(Name) && p.UserId == user.Id);
                if (val == null)
                {
                    FuturesAssistantWPF.Models.Parameter par = new FuturesAssistantWPF.Models.Parameter();
                    par.Id = Guid.NewGuid();
                    par.Name = Name;
                    par.UserId = user.Id;
                    par.Value = value;
                    statement.AddParameter(par);
                }
                else
                {
                    val.Value = value;
                    statement.EditParameter(val);
                }
                statement.SaveChanged();
            }
        }

        /// <summary>
        /// 从Parameter中取得参数值，如果Parameter中没有此参数，则返回NULL。
        /// </summary>
        /// <param name="Name">要查询的参数</param>
        /// <returns>取得的参数值</returns>
        public static string GetParameter(string Name)
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Models.Parameter)))
            {
                var val = statement.Parameters.FirstOrDefault(p => p.Name.Equals(Name) && p.UserId == _Session.LoginedUserId);
                if (val == null)
                {
                    return null;
                }
                else
                {
                    return val.Value;
                }
            }
        }

        /// <summary>
        /// 从Parameter中取得参数值，如果Parameter中没有此参数，则向Parameter中添加此参数，并设置参数值为defaultValue。
        /// 注：此方法已经自动设置AccountId参数
        /// </summary>
        /// <param name="Name">要查询的参数</param>
        /// <param name="defaultValue">如果Parameter中没有要查询的参数，则向Parameter中添加此参数时设置默认值</param>
        /// <returns>取得的参数值</returns>
        public static string GetParameter(string Name, string defaultValue)
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Models.Parameter)))
            {
                var val = statement.Parameters.FirstOrDefault(p => p.Name.Equals(Name) && p.UserId == _Session.LoginedUserId);
                if (val == null)
                {
                    FuturesAssistantWPF.Models.Parameter par = new FuturesAssistantWPF.Models.Parameter();
                    par.Id = Guid.NewGuid();
                    par.Name = Name;
                    par.UserId = _Session.LoginedUserId;
                    par.Value = defaultValue;
                    statement.AddParameter(par);
                    statement.SaveChanged();
                    return defaultValue;
                }
                else
                {
                    return val.Value;
                }
            }
        }

        /// <summary>
        /// 从Parameter中取得参数值，如果Parameter中没有此参数，则向Parameter中添加此参数，并设置参数值为defaultValue。
        /// </summary>
        /// <param name="Name">要查询的参数</param>
        /// <param name="defaultValue">如果Parameter中没有要查询的参数，则向Parameter中添加此参数时设置默认值</param>
        /// <param name="userId">此参数所属User</param>
        /// <returns>取得的参数值</returns>
        public static string GetParameter(string Name, string defaultValue, User user)
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Models.Parameter)))
            {
                var val = statement.Parameters.FirstOrDefault(p => p.Name.Equals(Name) && p.UserId == user.Id);
                if (val == null)
                {
                    FuturesAssistantWPF.Models.Parameter par = new FuturesAssistantWPF.Models.Parameter();
                    par.Id = Guid.NewGuid();
                    par.Name = Name;
                    par.UserId = user.Id;
                    par.Value = defaultValue;
                    statement.AddParameter(par);
                    statement.SaveChanged();
                    return defaultValue;
                }
                else
                {
                    return val.Value;
                }
            }
        }

        public static string parseCustomerNameFromHtml(string html)
        {
            //基本资料
            int startIndex = html.IndexOf("基本资料");
            int endIndex = html.IndexOf("资金状况");
            string strAccount = html.Substring(startIndex, endIndex - startIndex);
            endIndex = strAccount.IndexOf("</table>", StringComparison.CurrentCultureIgnoreCase);
            strAccount = strAccount.Substring(0, endIndex);

            //客户名称
            startIndex = strAccount.IndexOf("客户名称");
            strAccount = strAccount.Substring(startIndex);
            startIndex = strAccount.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
            strAccount = strAccount.Substring(startIndex);
            startIndex = strAccount.IndexOf(">") + 1;
            strAccount = strAccount.Substring(startIndex);
            endIndex = strAccount.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
            //
            return strAccount.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
        }

        public static string LoginWithVerify(CookieContainer cookie, string accountNumber, string password, string identifyingCode)
        {
            try
            {
                string url = "https://investorservice.cfmmc.com/login.do";
                string postData = string.Format("userID={0}&password={1}&vericode={2}", accountNumber, password, identifyingCode);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.Timeout = TimeOut;
                StreamWriter sw = new StreamWriter(request.GetRequestStream());
                sw.Write(postData);
                sw.Flush();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();
                sw.Dispose();
                sw.Close();
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                if (result.IndexOf("验证码错误") != -1)
                {
                    throw new IdentifyingCodeMismatchException();
                }
                else if (result.IndexOf("用户名或密码错误") != -1)
                {
                    throw new UsernameOrPasswordWrongException();
                }
                else if (result.IndexOf("输入信息不能为空") != -1)
                {
                    throw new InputNullException();
                }
                else if (result.IndexOf("您错误尝试超过3次") != -1)
                {
                    throw new TryTooMoreException();
                }
                else if (!_Session.CUSTOMER_NAME.Contains(_Helper.parseCustomerNameFromHtml(result)))
                {
                    throw new IllegalCustomerNameException();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Login(CookieContainer cookie, string accountNumber, string password, string identifyingCode)
        {
            try
            {
                string url = "https://investorservice.cfmmc.com/login.do";
                string postData = string.Format("userID={0}&password={1}&vericode={2}", accountNumber, password, identifyingCode);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.Timeout = TimeOut;
                StreamWriter sw = new StreamWriter(request.GetRequestStream());
                sw.Write(postData);
                sw.Flush();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();
                sw.Dispose();
                sw.Close();
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                if (result.IndexOf("验证码错误") != -1)
                {
                    throw new IdentifyingCodeMismatchException();
                }
                else if (result.IndexOf("用户名或密码错误") != -1)
                {
                    throw new UsernameOrPasswordWrongException();
                }
                else if (result.IndexOf("输入信息不能为空") != -1)
                {
                    throw new InputNullException();
                }
                else if (result.IndexOf("您错误尝试超过3次") != -1)
                {
                    throw new TryTooMoreException();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 登陆的前提下，转到《基本信息》页面。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RequestElementrayInformationPage(CookieContainer cookie)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create("https://investorservice.cfmmc.com/customer/setupViewCustomerDetail.do") as HttpWebRequest;
                request.Method = "get";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 5000;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                if (result.Contains("请重新登录"))
                {
                    throw new NoneOnlineAccountException();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void TouchCmffc(CookieContainer cookie)
        {
            try
            {
                string url = "https://investorservice.cfmmc.com";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Timeout = TimeOut;
                request.Timeout = TimeOut;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                request.Abort();
                response.GetResponseStream().Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("连接保证金监控中心失败，请保证网络畅通，错误信息：\n{0}", ex.Message));
            }
        }

        public static Image GetValidateCode(CookieContainer cookie)
        {
            try
            {
                //
                TouchCmffc(cookie);

                //
                string url = "https://investorservice.cfmmc.com/veriCode.do?t=1357198032689&ip=42.63.201.225";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Timeout = TimeOut;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                Image result = Image.FromStream(s);
                s.Dispose();
                s.Close();
                request.Abort();
                response.Close();
                return result;
            }
            catch (WebException ex)
            {
                throw new Exception(string.Format("获取保证金监控中心验证码失败，请保证网络畅通，错误信息：\n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static List<PositionDetail> ParseHtmlPositionDetails(DateTime statementDate, string htmlPositionDetails, StatementContext statement, Guid accountId)
        {
            List<PositionDetail> positionDetails = new List<PositionDetail>();
            try
            {
                //
                SettlementType settlementType = htmlPositionDetails.Contains("逐日盯市") ? SettlementType.date : SettlementType.trade;

                //
                int startIndex = htmlPositionDetails.IndexOf("持仓明细");
                int endIndex = htmlPositionDetails.IndexOf("</table>", htmlPositionDetails.IndexOf("实际成交日期"), StringComparison.CurrentCultureIgnoreCase);
                string strPositionDetails = htmlPositionDetails.Substring(startIndex, endIndex - startIndex);

                //逐行读取
                //跳过标题
                startIndex = strPositionDetails.IndexOf(@"<TR", strPositionDetails.IndexOf("实际成交日期"), StringComparison.CurrentCultureIgnoreCase);
                startIndex = strPositionDetails.IndexOf(@"<Td", startIndex, StringComparison.CurrentCultureIgnoreCase);
                while (true)
                {
                    //
                    PositionDetail positionDetail = new PositionDetail();
                    positionDetail.Id = Guid.NewGuid();
                    positionDetail.AccountId = accountId;
                    positionDetail.DateForPosition = statementDate;

                    //合约
                    strPositionDetails = strPositionDetails.Substring(startIndex);
                    startIndex = strPositionDetails.IndexOf(">") + 1;
                    strPositionDetails = strPositionDetails.Substring(startIndex);
                    endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strItem = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strItem.Equals("合计"))
                    {
                        positionDetail.Item = strItem;

                        //成交序号
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        positionDetail.Ticket = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //买持仓（手数）
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strBuySize = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (!string.IsNullOrEmpty(strBuySize))
                            positionDetail.BuySize = Convert.ToInt32(strBuySize);

                        //买入价
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strBuyPrice = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (!string.IsNullOrEmpty(strBuyPrice))
                            positionDetail.BuyPrice = Convert.ToDecimal(strBuyPrice);

                        //卖持仓（手数）
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSaleSize = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (!string.IsNullOrEmpty(strSaleSize))
                            positionDetail.SaleSize = Convert.ToInt32(strSaleSize);

                        //卖出价
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSalePrice = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (!string.IsNullOrEmpty(strSalePrice))
                            positionDetail.SalePrice = Convert.ToDecimal(strSalePrice);

                        //昨结算价
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strYesterdaySettlementPrice = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        try
                        {
                            positionDetail.YesterdaySettlementPrice = Convert.ToDecimal(strYesterdaySettlementPrice);
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        //今结算价
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strTodaySettlementPrice = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        positionDetail.TodaySettlementPrice = Convert.ToDecimal(strTodaySettlementPrice);

                        //逐笔对冲：浮动盈亏 / 逐日盯市：持仓盈亏
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strFloatingProfit = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        positionDetail.Profit = Convert.ToDecimal(strFloatingProfit);

                        //投机/套保
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        positionDetail.SH = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //交易编码
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        positionDetail.TradeCode = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //实际成交日期
                        startIndex = strPositionDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        startIndex = strPositionDetails.IndexOf(">") + 1;
                        strPositionDetails = strPositionDetails.Substring(startIndex);
                        endIndex = strPositionDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strDate = strPositionDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        positionDetail.DateForActual = Convert.ToDateTime(strDate);

                        //结算方式
                        positionDetail.SettlementType = settlementType;

                        //
                        positionDetails.Add(positionDetail);
                        //statement.AddPositionDetail(positionDetail);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strPositionDetails.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strPositionDetails.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return positionDetails;
        }

        private static List<ClosedTradeDetail> ParseHtmlClosedTradeDetails(DateTime statementDate, string htmlClosedTradeDetails, StatementContext statement, Guid accountId)
        {
            List<ClosedTradeDetail> closedTradeDetails = new List<ClosedTradeDetail>();
            try
            {
                int startIndex = htmlClosedTradeDetails.IndexOf("平仓明细");
                int endIndex = htmlClosedTradeDetails.IndexOf("</table>", htmlClosedTradeDetails.IndexOf("实际成交日期"), StringComparison.CurrentCultureIgnoreCase);
                string strClosedTradeDetails = htmlClosedTradeDetails.Substring(startIndex, endIndex - startIndex);

                //逐行读取
                //跳过标题
                startIndex = strClosedTradeDetails.IndexOf(@"<TD", strClosedTradeDetails.IndexOf("实际成交日期"), StringComparison.CurrentCultureIgnoreCase);
                while (true)
                {
                    //
                    ClosedTradeDetail closedTradeDetail = new ClosedTradeDetail();
                    closedTradeDetail.Id = Guid.NewGuid();
                    closedTradeDetail.AccountId = accountId;

                    //合约
                    strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                    startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                    strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                    endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strItem = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strItem.Equals("合计"))
                    {
                        closedTradeDetail.Item = strItem;
                        //成交序号
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        closedTradeDetail.TicketForClose = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //买/卖
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        closedTradeDetail.BS = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //成交价
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strPriceForClose = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.PriceForClose = Convert.ToDecimal(strPriceForClose);

                        //开仓价
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strPriceForOpen = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.PriceForOpen = Convert.ToDecimal(strPriceForOpen);

                        //手数
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSize = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.Size = Convert.ToInt32(strSize);

                        //昨日结算价
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strYesterdaySettlementPrice = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.YesterdaySettlementPrice = Convert.ToDecimal(strYesterdaySettlementPrice);

                        //平仓盈亏
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strClosedProfit = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.ClosedProfit = Convert.ToDecimal(strClosedProfit);

                        //原成交序号
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        closedTradeDetail.TicketForOpen = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //实际成交日期
                        startIndex = strClosedTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        startIndex = strClosedTradeDetails.IndexOf(">") + 1;
                        strClosedTradeDetails = strClosedTradeDetails.Substring(startIndex);
                        endIndex = strClosedTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strDate = strClosedTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        closedTradeDetail.ActualDate = Convert.ToDateTime(strDate);

                        closedTradeDetails.Add(closedTradeDetail);
                        //statement.AddClosedTradeDetail(closedTradeDetail);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strClosedTradeDetails.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strClosedTradeDetails.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return closedTradeDetails;
        }

        private static List<TradeDetail> ParseHtmlTradeDetails(DateTime statementDate, string htmlTradeDetails, StatementContext statement, Guid accountId)
        {
            List<TradeDetail> tradeDetails = new List<TradeDetail>();
            try
            {
                int startIndex = htmlTradeDetails.IndexOf("成交明细");
                int endIndex = htmlTradeDetails.IndexOf("</table>", htmlTradeDetails.IndexOf("平仓盈亏"), StringComparison.CurrentCultureIgnoreCase);
                string strTradeDetails = htmlTradeDetails.Substring(startIndex, endIndex - startIndex);

                //逐行读取
                //跳过标题
                startIndex = strTradeDetails.IndexOf(@"<TD", strTradeDetails.IndexOf("实际成交日期"), StringComparison.CurrentCultureIgnoreCase);
                while (true)
                {
                    //
                    TradeDetail tradeDetail = new TradeDetail();
                    tradeDetail.Id = Guid.NewGuid();
                    tradeDetail.AccountId = accountId;

                    //合约
                    strTradeDetails = strTradeDetails.Substring(startIndex);
                    startIndex = strTradeDetails.IndexOf(">") + 1;
                    strTradeDetails = strTradeDetails.Substring(startIndex);
                    endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strItem = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strItem.Equals("合计"))
                    {
                        tradeDetail.Item = strItem;

                        //成交序号
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        tradeDetail.Ticket = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //成交时间
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strTime = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //买/卖
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        tradeDetail.BS = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //投机/套保
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        tradeDetail.SH = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //成交价
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strPrice = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        tradeDetail.Price = Convert.ToDecimal(strPrice);

                        //手数
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSize = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        tradeDetail.Size = Convert.ToInt32(strSize);

                        //成交额
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strAmount = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        tradeDetail.Amount = Convert.ToDecimal(strAmount);

                        //开/平
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        tradeDetail.OC = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //手续费
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strCommission = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        tradeDetail.Commission = string.IsNullOrEmpty(strCommission) ? 0 : Convert.ToDecimal(strCommission);

                        //平仓盈亏
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strClosedProfit = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (strClosedProfit.Equals("--"))
                        {
                            tradeDetail.ClosedProfit = 0;
                        }
                        else
                        {
                            tradeDetail.ClosedProfit = Convert.ToDecimal(strClosedProfit);
                        }

                        //实际成交日期
                        startIndex = strTradeDetails.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        startIndex = strTradeDetails.IndexOf(">") + 1;
                        strTradeDetails = strTradeDetails.Substring(startIndex);
                        endIndex = strTradeDetails.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strDate = strTradeDetails.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        //  strDate="2013-11-25"   strTime="09:00:01"
                        tradeDetail.ActualTime = Convert.ToDateTime(strDate + " " + strTime);

                        tradeDetails.Add(tradeDetail);
                        //statement.AddTradeDetail(tradeDetail);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strTradeDetails.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strTradeDetails.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return tradeDetails;
        }

        private static List<CommoditySummarization> ParseHtmlCommoditySummarizations(DateTime statementDate, string htmlCommoditySummarization, StatementContext statement, Guid accountId)
        {
            List<CommoditySummarization> commoditySummarizations = new List<CommoditySummarization>();
            try
            {
                int startIndex = htmlCommoditySummarization.IndexOf("品种汇总");
                int endIndex = htmlCommoditySummarization.IndexOf("</table>", htmlCommoditySummarization.IndexOf("平仓盈亏"), StringComparison.CurrentCultureIgnoreCase);
                string strCommoditySummarizations = htmlCommoditySummarization.Substring(startIndex, endIndex - startIndex);

                //逐行读取
                //跳过标题
                startIndex = strCommoditySummarizations.IndexOf(@"<TD", strCommoditySummarizations.IndexOf("平仓盈亏"), StringComparison.CurrentCultureIgnoreCase);
                while (true)
                {
                    //
                    CommoditySummarization commoditySummarization = new CommoditySummarization();
                    commoditySummarization.Id = Guid.NewGuid();
                    commoditySummarization.AccountId = accountId;
                    commoditySummarization.Date = statementDate;

                    //品种
                    strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                    startIndex = strCommoditySummarizations.IndexOf(">") + 1;
                    strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                    endIndex = strCommoditySummarizations.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strCommodity = strCommoditySummarizations.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strCommodity.Equals("合计"))
                    {
                        commoditySummarization.Commodity = strCommodity;

                        //手数
                        startIndex = strCommoditySummarizations.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        startIndex = strCommoditySummarizations.IndexOf(">") + 1;
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        endIndex = strCommoditySummarizations.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSize = strCommoditySummarizations.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        commoditySummarization.Size = Convert.ToInt32(strSize);

                        //成交额
                        startIndex = strCommoditySummarizations.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        startIndex = strCommoditySummarizations.IndexOf(">") + 1;
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        endIndex = strCommoditySummarizations.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strAmount = strCommoditySummarizations.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        commoditySummarization.Amount = Convert.ToDecimal(strAmount);

                        //手续费
                        startIndex = strCommoditySummarizations.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        startIndex = strCommoditySummarizations.IndexOf(">") + 1;
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        endIndex = strCommoditySummarizations.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strCommission = strCommoditySummarizations.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        commoditySummarization.Commission = Convert.ToDecimal(strCommission);


                        //平仓盈亏
                        startIndex = strCommoditySummarizations.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        startIndex = strCommoditySummarizations.IndexOf(">") + 1;
                        strCommoditySummarizations = strCommoditySummarizations.Substring(startIndex);
                        endIndex = strCommoditySummarizations.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strClosedProfit = strCommoditySummarizations.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        commoditySummarization.ClosedProfit = Convert.ToDecimal(strClosedProfit);

                        commoditySummarizations.Add(commoditySummarization);
                        //statement.AddCommoditySummarization(commoditySummarization);

                        using (StatementContext context = new StatementContext())
                        {
                            if (context.Commoditys.FirstOrDefault(m => m.Code.ToLower().Equals(commoditySummarization.Commodity.ToLower())) == null)
                            {
                                Commodity cd = new Commodity();
                                cd.Code = commoditySummarization.Commodity;
                                cd.Name = commoditySummarization.Commodity;
                                context.AddCommodity(cd);
                                context.SaveChanged();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strCommoditySummarizations.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strCommoditySummarizations.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return commoditySummarizations;
        }

        private static void ParseHtmlStatementHomePage(DateTime statementDate, string htmlStatement, StatementContext statement,
            out FundStatus fundStatus, out List<Remittance> remittances, out List<Trade> trades, out List<Position> positions, out decimal? amount, Guid accountId)
        {
            try
            {
                //基本资料
                #region
                int startIndex = htmlStatement.IndexOf("基本资料");
                int endIndex = htmlStatement.IndexOf("资金状况");
                string strAccount = htmlStatement.Substring(startIndex, endIndex - startIndex);
                endIndex = strAccount.IndexOf("</table>", StringComparison.CurrentCultureIgnoreCase);
                strAccount = strAccount.Substring(0, endIndex);
                #endregion

                //资金状况
                #region
                fundStatus = new FundStatus();
                fundStatus.Id = Guid.NewGuid();
                fundStatus.AccountId = accountId;

                //
                startIndex = htmlStatement.IndexOf("资金状况");
                endIndex = htmlStatement.IndexOf("出入金明细");
                string strFundStatus = htmlStatement.Substring(startIndex, endIndex - startIndex);
                endIndex = strFundStatus.IndexOf("</table>", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(0, endIndex);

                //交易日期
                fundStatus.Date = statementDate;

                //上日结存
                startIndex = strFundStatus.IndexOf("上日结存");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.YesterdayBalance = 0;
                else
                    fundStatus.YesterdayBalance = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //客户权益
                startIndex = strFundStatus.IndexOf("客户权益");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.CustomerRights = 0;
                else
                    fundStatus.CustomerRights = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //当日存取合计
                startIndex = strFundStatus.IndexOf("当日存取合计");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.Remittance = 0;
                else
                    fundStatus.Remittance = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //平仓盈亏
                startIndex = strFundStatus.IndexOf("平仓盈亏");
                if (startIndex != -1)
                {
                    //逐笔对冲
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(">") + 1;
                    strFundStatus = strFundStatus.Substring(startIndex);
                    endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                        fundStatus.ClosedProfit = 0;
                    else
                        fundStatus.ClosedProfit = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());
                }
                else
                {
                    //逐日盯市（显示为 “当日盈亏”）
                    startIndex = strFundStatus.IndexOf("当日盈亏");
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(">") + 1;
                    strFundStatus = strFundStatus.Substring(startIndex);
                    endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                        fundStatus.ClosedProfit = 0;
                    else
                        fundStatus.ClosedProfit = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());
                }

                //当日手续费
                startIndex = strFundStatus.IndexOf("当日手续费");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.Commission = 0;
                else
                    fundStatus.Commission = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //冻结资金
                startIndex = strFundStatus.IndexOf("冻结资金");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.MatterDeposit = 0;
                else
                    fundStatus.MatterDeposit = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //当日结存
                startIndex = strFundStatus.IndexOf("当日结存");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.TodayBalance = 0;
                else
                    fundStatus.TodayBalance = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //保证金占用
                startIndex = strFundStatus.IndexOf("保证金占用");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.Margin = 0;
                else
                    fundStatus.Margin = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());



                //可用资金
                startIndex = strFundStatus.IndexOf("可用资金");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.FreeMargin = 0;
                else
                    fundStatus.FreeMargin = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());


                //风险度
                startIndex = strFundStatus.IndexOf("风险度");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"%");
                string tmp = strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                fundStatus.VentureFactor = Convert.ToDouble(tmp.Contains("--") ? "0" : tmp);

                //浮动盈亏
                startIndex = strFundStatus.IndexOf("浮动盈亏");
                if (startIndex != -1)
                {
                    //逐笔对冲
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                    strFundStatus = strFundStatus.Substring(startIndex);
                    startIndex = strFundStatus.IndexOf(">") + 1;
                    strFundStatus = strFundStatus.Substring(startIndex);
                    endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                        fundStatus.FloatingProfit = 0;
                    else
                        fundStatus.FloatingProfit = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());
                    fundStatus.SettlementType = SettlementType.trade;

                }
                else
                {
                    //逐日盯市（不存在“浮动盈亏”项目）
                    fundStatus.SettlementType = SettlementType.date;
                }

                //追加保证金
                startIndex = strFundStatus.IndexOf("追加保证金");
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                strFundStatus = strFundStatus.Substring(startIndex);
                startIndex = strFundStatus.IndexOf(">") + 1;
                strFundStatus = strFundStatus.Substring(startIndex);
                endIndex = strFundStatus.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                if (strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim().Equals("--"))
                    fundStatus.AdditionalMargin = 0;
                else
                    fundStatus.AdditionalMargin = Convert.ToDecimal(strFundStatus.Substring(0, endIndex).Replace("&nbsp;", "").Trim());

                //

                //statement.AddFundStatus(fundStatus);
                #endregion

                //出入金明细
                #region
                startIndex = htmlStatement.IndexOf("出入金明细");
                endIndex = htmlStatement.IndexOf("成交汇总");
                string strRemittances = htmlStatement.Substring(startIndex, endIndex - startIndex);
                endIndex = strRemittances.IndexOf(@"</table>", StringComparison.CurrentCultureIgnoreCase);
                strRemittances = strRemittances.Substring(0, endIndex);

                //逐行读取
                //跳过标题
                startIndex = strRemittances.IndexOf(@"<TD", strRemittances.IndexOf("摘要"), StringComparison.CurrentCultureIgnoreCase);
                remittances = new List<Remittance>();
                while (true)
                {
                    //
                    Remittance remittance = new Remittance();
                    remittance.Id = Guid.NewGuid();
                    remittance.AccountId = accountId;

                    //发生日期
                    strRemittances = strRemittances.Substring(startIndex);
                    startIndex = strRemittances.IndexOf(">") + 1;
                    strRemittances = strRemittances.Substring(startIndex);
                    endIndex = strRemittances.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strDate = strRemittances.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strDate.Equals("合计"))
                    {
                        remittance.Date = Convert.ToDateTime(strDate);

                        //入金
                        startIndex = strRemittances.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strRemittances = strRemittances.Substring(startIndex);
                        startIndex = strRemittances.IndexOf(">") + 1;
                        strRemittances = strRemittances.Substring(startIndex);
                        endIndex = strRemittances.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strDeposit = strRemittances.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        remittance.Deposit = string.IsNullOrEmpty(strDeposit) ? 0 : Convert.ToDecimal(strDeposit);

                        //出金
                        startIndex = strRemittances.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strRemittances = strRemittances.Substring(startIndex);
                        startIndex = strRemittances.IndexOf(">") + 1;
                        strRemittances = strRemittances.Substring(startIndex);
                        endIndex = strRemittances.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strWithDrawal = strRemittances.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        remittance.WithDrawal = string.IsNullOrEmpty(strWithDrawal) ? 0 : Convert.ToDecimal(strWithDrawal);

                        //方式
                        startIndex = strRemittances.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strRemittances = strRemittances.Substring(startIndex);
                        startIndex = strRemittances.IndexOf(">") + 1;
                        strRemittances = strRemittances.Substring(startIndex);
                        endIndex = strRemittances.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        remittance.Type = strRemittances.Substring(0, endIndex).Replace("&nbsp;", "").Trim();


                        //摘要
                        startIndex = strRemittances.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strRemittances = strRemittances.Substring(startIndex);
                        startIndex = strRemittances.IndexOf(">") + 1;
                        strRemittances = strRemittances.Substring(startIndex);
                        endIndex = strRemittances.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        remittance.Summary = strRemittances.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //加入出入金列表
                        remittances.Add(remittance);
                        //statement.AddRemittance(remittance);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strRemittances.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strRemittances.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);

                }
                #endregion

                //成交汇总
                #region
                startIndex = htmlStatement.IndexOf("成交汇总");
                endIndex = htmlStatement.IndexOf("持仓汇总");
                string strTrades = htmlStatement.Substring(startIndex, endIndex - startIndex);
                endIndex = strTrades.IndexOf("</table>", strTrades.IndexOf("平仓盈亏"), StringComparison.CurrentCultureIgnoreCase);
                strTrades = strTrades.Substring(0, endIndex);

                //逐行读取
                //跳过标题
                amount = 0;
                startIndex = strTrades.IndexOf(@"<TD", strTrades.IndexOf("平仓盈亏"), StringComparison.CurrentCultureIgnoreCase);
                trades = new List<Trade>();
                while (true)
                {
                    //
                    Trade trade = new Trade();
                    trade.Id = Guid.NewGuid();
                    trade.AccountId = accountId;
                    trade.Date = statementDate;

                    //合约
                    strTrades = strTrades.Substring(startIndex);
                    startIndex = strTrades.IndexOf(">") + 1;
                    strTrades = strTrades.Substring(startIndex);
                    endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strItem = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strItem.Equals("合计"))
                    {
                        trade.Item = strItem;

                        //买/卖
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        trade.BS = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();


                        //投机/套保
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        trade.SH = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //成交价
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strPrice = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        trade.Price = string.IsNullOrEmpty(strPrice) ? 0 : Convert.ToDecimal(strPrice);


                        //手数
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSize = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        trade.Size = string.IsNullOrEmpty(strSize) ? 0 : Convert.ToInt32(strSize);

                        //成交额
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strAmount = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        amount += trade.Amount = string.IsNullOrEmpty(strAmount) ? 0 : Convert.ToDecimal(strAmount);

                        //开/平
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        trade.OC = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //手续费
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strCommission = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        trade.Commission = string.IsNullOrEmpty(strCommission) ? 0 : Convert.ToDecimal(strCommission);

                        //平仓盈亏
                        startIndex = strTrades.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strTrades = strTrades.Substring(startIndex);
                        startIndex = strTrades.IndexOf(">") + 1;
                        strTrades = strTrades.Substring(startIndex);
                        endIndex = strTrades.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strClosedProfit = strTrades.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        if (strClosedProfit.Equals("--") || string.IsNullOrEmpty(strClosedProfit))
                        {
                            trade.ClosedProfit = 0;
                        }
                        else
                        {
                            trade.ClosedProfit = Convert.ToDecimal(strClosedProfit);
                        }


                        //
                        trades.Add(trade);
                        //statement.AddTrade(trade);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strTrades.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strTrades.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);

                }
                #endregion

                //持仓汇总 (追加保证金通知)
                #region
                startIndex = htmlStatement.IndexOf("持仓汇总");
                endIndex = htmlStatement.IndexOf(@"</html>", StringComparison.CurrentCultureIgnoreCase);
                string strPositions = htmlStatement.Substring(startIndex, endIndex - startIndex);
                endIndex = strPositions.IndexOf("</table>", strPositions.IndexOf("投机/套保"), StringComparison.CurrentCultureIgnoreCase);
                strPositions = strPositions.Substring(0, endIndex);

                //
                SettlementType settlementType = strPositions.Contains("持仓盈亏") ? SettlementType.date : SettlementType.trade;

                //逐行读取
                //跳过标题
                startIndex = strPositions.IndexOf(@"<TR", strPositions.IndexOf("投机/套保"), StringComparison.CurrentCultureIgnoreCase);
                startIndex = strPositions.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);
                positions = new List<Position>();
                while (true)
                {
                    //
                    Position position = new Position();
                    position.Id = Guid.NewGuid();
                    position.AccountId = accountId;
                    position.Date = statementDate;

                    //合约      
                    strPositions = strPositions.Substring(startIndex);
                    startIndex = strPositions.IndexOf(">") + 1;
                    strPositions = strPositions.Substring(startIndex);
                    endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                    string strItem = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                    if (!strItem.Equals("合计"))
                    {
                        position.Item = strItem;

                        //买持仓
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strBuySize = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        try
                        {
                            if (!string.IsNullOrEmpty(strBuySize))
                                position.BuySize = Convert.ToInt32(strBuySize);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                        //买均价
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strBuyAveragePrice = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        if (!string.IsNullOrEmpty(strBuyAveragePrice))
                            position.BuyAveragePrice = Convert.ToDecimal(strBuyAveragePrice);

                        //卖持仓
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSaleSize = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        if (!string.IsNullOrEmpty(strSaleSize))
                            position.SaleSize = Convert.ToInt32(strSaleSize);

                        //卖均价
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strSaleAveragePrice = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        if (!string.IsNullOrEmpty(strSaleAveragePrice))
                            position.SaleAveragePrice = Convert.ToDecimal(strSaleAveragePrice);

                        //昨结算价
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strYesterdaySettlementPrice = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        position.YesterdaySettlementPrice = string.IsNullOrEmpty(strYesterdaySettlementPrice) ? 0 : Convert.ToDecimal(strYesterdaySettlementPrice);

                        //今结算价
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strTodaySettlementPrice = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        position.TodaySettlementPrice = string.IsNullOrEmpty(strTodaySettlementPrice) ? 0 : Convert.ToDecimal(strTodaySettlementPrice);

                        //盈亏
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strProfit = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        position.Profit = string.IsNullOrEmpty(strProfit) ? 0 : Convert.ToDecimal(strProfit);

                        //  交易保证金 
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        string strMargin = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Replace("-", "").Trim();
                        position.Margin = string.IsNullOrEmpty(strMargin) ? 0 : Convert.ToDecimal(strMargin);

                        //投机/套保 
                        startIndex = strPositions.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strPositions = strPositions.Substring(startIndex);
                        startIndex = strPositions.IndexOf(">") + 1;
                        strPositions = strPositions.Substring(startIndex);
                        endIndex = strPositions.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        position.SH = strPositions.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //结算方式
                        position.SettlementType = settlementType;

                        //
                        positions.Add(position);
                        //statement.AddPosition(position);
                    }
                    else
                    {
                        break;
                    }

                    //下一行数据
                    startIndex = strPositions.IndexOf(@"<TR", StringComparison.CurrentCultureIgnoreCase);
                    startIndex = strPositions.IndexOf(@"<TD", startIndex, StringComparison.CurrentCultureIgnoreCase);

                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                throw;
            }
        }


        public static void GetStatementByRequestHtml(CookieContainer cookie, DateTime tradeDate, SettlementType settlementType, StatementContext statement,
            out decimal? yesterdayBalance, out decimal? todayBalance, out decimal? remittance, out decimal? amount, Guid accountId)
        {

            amount = null;
            yesterdayBalance = null;
            todayBalance = null;
            remittance = null;

            //var days = (DateTime.Today - tradeDate.Date).TotalDays;
            // 不更新今天的数据
            //if (days == 0)
            //{
            //    return;
            //}

            // 相对今天：前一个交易日的数据，需先判断今天的数据是否已经报送完毕。
            //if (tradeDate.DayOfWeek == DayOfWeek.Friday)
            //{
            //    if (days < 3)
            //    {
            //        return;
            //    }
            //    if (days == 3)
            //    {
            //        if (!IsFinishedStatementData(cookie, tradeDate.AddDays(3), settlementType, statement, accountId))
            //        {
            //            return;
            //        }
            //    }
            //}
            //else if (days == 1)
            //{
            //    if (!IsFinishedStatementData(cookie, tradeDate.AddDays(1), settlementType, statement, accountId))
            //    {
            //        return;
            //    }
            //}

            try
            {

                #region 发送请求
                //设置参数
                string url = "https://investorservice.cfmmc.com/customer/setParameter.do";
                string postData = string.Format("tradeDate={0}&byType={1}", tradeDate.ToString("yyyy-MM-dd"), settlementType.ToString());

                //请求
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.ProtocolVersion = httpVersion;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                request.Timeout = TimeOut;
                request.KeepAlive = false;
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write(postData);
                writer.Flush();
                //回应
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string htmlStatementHomePage = reader.ReadToEnd();
                request.Abort();
                response.Close();

                //
                writer.Dispose();
                writer.Close();
                reader.Dispose();
                reader.Close();
                stream.Dispose();
                stream.Close();
                #endregion

                FundStatus fundStatus;
                List<Remittance> remittances;
                List<Trade> trades;
                List<Position> positions;
                List<CommoditySummarization> commoditySummarizations;
                List<TradeDetail> tradeDetails;
                List<ClosedTradeDetail> closedTradeDetails;
                List<PositionDetail> positionDetails;


                //是否在查询期限内： 查询系统目前不提供从今日起六个月之前的数据查询服务，请将查询日期限定在六个月之内。
                //是否为交易日：   2015-08-16为非交易日，请重新选择交易日期。
                //当天的数据是否已报送：   系统中无投资者01778661001198(王世起)在2015-08-17的交易结算报告，原因是期货公司未向监控中心报送该日数据。
                if (!string.IsNullOrEmpty(htmlStatementHomePage) && htmlStatementHomePage.IndexOf("查询系统目前不提供从今日起") == -1 && htmlStatementHomePage.IndexOf("系统中无投资者") == -1 && htmlStatementHomePage.IndexOf("为非交易日，请重新选择交易日期") == -1)
                {
                    //
                    ParseHtmlStatementHomePage(tradeDate, htmlStatementHomePage, statement, out fundStatus, out remittances, out trades, out positions, out amount, accountId);

                    //
                    yesterdayBalance = fundStatus.YesterdayBalance;
                    todayBalance = fundStatus.TodayBalance;
                    remittance = fundStatus.Remittance;

                    //下载Excel数据
                    LoadExcelStatement(cookie, tradeDate, settlementType, accountId);

                    // 品种汇总
                    commoditySummarizations = GetCommoditySummarizationsFromHtml(cookie, tradeDate, statement, accountId);

                    // 成交明细
                    tradeDetails = GetTradeDetailsFromHtml(cookie, tradeDate, statement, accountId);

                    // 平仓明细
                    closedTradeDetails = GetClosedTradeDetailsFromHtml(cookie, tradeDate, statement, accountId);

                    // 持仓明细
                    positionDetails = GetPositionDetailsFromHtml(cookie, tradeDate, statement, accountId);


                    // 检查当天报表中明细数据是否完整。
                    if (!((fundStatus.Commission != 0 && tradeDetails.Count == 0) || (fundStatus.Remittance != 0 && remittances.Count == 0)))
                    {
                        statement.AddFundStatus(fundStatus);
                        statement.AddRemittances(remittances);
                        statement.AddTrades(trades);
                        statement.AddPositions(positions);
                        statement.AddCommoditySummarizations(commoditySummarizations);
                        statement.AddTradeDetails(tradeDetails);
                        statement.AddClosedTradeDetails(closedTradeDetails);
                    }
                }
                //
                tryCount = 0;
            }
            catch (IOException)
            {
                if (tryCount <= 10)
                {
                    GetStatementByLoadExcel(cookie, tradeDate, settlementType, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, accountId);
                    tryCount++;
                }
                else
                {
                    MessageBox.Show("网络IO读取多次失败，请运行更新程序。");
                    throw;
                }
            }
            catch (WebException)
            {
                if (tryCount <= 10)
                {
                    GetStatementByLoadExcel(cookie, tradeDate, settlementType, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, accountId);
                    tryCount++;
                }
                else
                {
                    MessageBox.Show("连接监控中心时因网络问题多次中断，请保证网络畅通后，再次运行更新程序。");
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
        public static bool IsFinishedStatementData(CookieContainer cookie, DateTime tradeDate, SettlementType settlementType, StatementContext statement, Guid accountId)
        {
            #region 发送请求
            //设置参数
            string url = "https://investorservice.cfmmc.com/customer/setParameter.do";
            string postData = string.Format("tradeDate={0}&byType={1}", tradeDate.ToString("yyyy-MM-dd"), settlementType.ToString());

            //请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.CookieContainer = cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.ProtocolVersion = httpVersion;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.Timeout = TimeOut;
            request.KeepAlive = false;
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(postData);
            writer.Flush();

            //回应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string htmlStatementHomePage = reader.ReadToEnd();
            request.Abort();
            response.Close();
            #endregion

            FundStatus fundStatus;
            List<Remittance> remittances;
            List<Trade> trades;
            List<Position> positions;
            List<CommoditySummarization> commoditySummarizations;
            List<TradeDetail> tradeDetails;
            List<ClosedTradeDetail> closedTradeDetails;
            List<PositionDetail> positionDetails;


            //是否在查询期限内： 查询系统目前不提供从今日起六个月之前的数据查询服务，请将查询日期限定在六个月之内。
            //是否为交易日：   2015-08-16为非交易日，请重新选择交易日期。
            //当天的数据是否已报送：   系统中无投资者01778661001198(王世起)在2015-08-17的交易结算报告，原因是期货公司未向监控中心报送该日数据。
            if (htmlStatementHomePage.IndexOf("查询系统目前不提供从今日起") == -1 && htmlStatementHomePage.IndexOf("系统中无投资者") == -1 && htmlStatementHomePage.IndexOf("为非交易日，请重新选择交易日期") == -1)
            {
                decimal? amount;
                //
                ParseHtmlStatementHomePage(tradeDate, htmlStatementHomePage, statement, out fundStatus, out remittances, out trades, out positions, out amount, accountId);

                //
                //yesterdayBalance = fundStatus.YesterdayBalance;
                //todayBalance = fundStatus.TodayBalance;
                //remittance = fundStatus.Remittance;

                // 品种汇总
                commoditySummarizations = GetCommoditySummarizationsFromHtml(cookie, tradeDate, statement, accountId);

                // 成交明细
                tradeDetails = GetTradeDetailsFromHtml(cookie, tradeDate, statement, accountId);

                // 平仓明细
                closedTradeDetails = GetClosedTradeDetailsFromHtml(cookie, tradeDate, statement, accountId);

                // 持仓明细
                positionDetails = GetPositionDetailsFromHtml(cookie, tradeDate, statement, accountId);

                //下载Excel数据
                //LoadExcelStatement(cookie, tradeDate, settlementType, accountId);

                //
                writer.Dispose();
                writer.Close();
                reader.Dispose();
                reader.Close();
                stream.Dispose();
                stream.Close();

                // 检查当天报表中明细数据是否完整。
                if ((fundStatus.Commission != 0 && tradeDetails.Count == 0) || (fundStatus.Remittance != 0 && remittances.Count == 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private static List<PositionDetail> GetPositionDetailsFromHtml(CookieContainer cookie, DateTime tradeDate, StatementContext statement, Guid accountId)
        {
            //设置参数
            string url = "https://investorservice.cfmmc.com/customer/setupViewPositionDetailAction.do";
            string postData = string.Format("name={0}&value={1}", "org.apache.struts.taglib.html.TOKEN", "");

            //请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.CookieContainer = cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.ProtocolVersion = httpVersion;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.Timeout = TimeOut;
            request.KeepAlive = false;
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(postData);
            writer.Flush();

            //回应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string htmlPositionDetails = reader.ReadToEnd();
            request.Abort();
            response.Close();
            if (string.IsNullOrEmpty(htmlPositionDetails))
            {
                return new List<PositionDetail>();
            }
            else
                return ParseHtmlPositionDetails(tradeDate, htmlPositionDetails, statement, accountId);
        }

        private static List<ClosedTradeDetail> GetClosedTradeDetailsFromHtml(CookieContainer cookie, DateTime tradeDate, StatementContext statement, Guid accountId)
        {
            //设置参数
            string url = "https://investorservice.cfmmc.com/customer/setupViewLiquidDetailAction.do";
            string postData = string.Format("name={0}&value={1}", "org.apache.struts.taglib.html.TOKEN", "");

            //请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.CookieContainer = cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.ProtocolVersion = httpVersion;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.Timeout = TimeOut;
            request.KeepAlive = false;
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(postData);
            writer.Flush();

            //回应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string htmlCloseTradeDetails = reader.ReadToEnd();
            request.Abort();
            response.Close();
            if (string.IsNullOrEmpty(htmlCloseTradeDetails))
            {
                return new List<ClosedTradeDetail>();
            }
            else
                return ParseHtmlClosedTradeDetails(tradeDate, htmlCloseTradeDetails, statement, accountId);
        }

        private static List<TradeDetail> GetTradeDetailsFromHtml(CookieContainer cookie, DateTime tradeDate, StatementContext statement, Guid accountId)
        {
            //设置参数
            string url = "https://investorservice.cfmmc.com/customer/setupViewTradeDetailAction.do";
            string postData = string.Format("name={0}&value={1}", "org.apache.struts.taglib.html.TOKEN", "");

            //请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.CookieContainer = cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.ProtocolVersion = httpVersion;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.Timeout = TimeOut;
            request.KeepAlive = false;
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(postData);
            writer.Flush();
            //回应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string htmlTradeDetails = reader.ReadToEnd();
            request.Abort();
            response.Close();
            if (string.IsNullOrEmpty(htmlTradeDetails))
            {
                return new List<TradeDetail>();
            }
            else
                return ParseHtmlTradeDetails(tradeDate, htmlTradeDetails, statement, accountId);
        }

        private static List<CommoditySummarization> GetCommoditySummarizationsFromHtml(CookieContainer cookie, DateTime tradeDate, StatementContext statement, Guid accountId)
        {
            //设置参数
            string url = "https://investorservice.cfmmc.com/customer/setupViewTradeCommodityAction.do";
            string postData = string.Format("name={0}&value={1}", "org.apache.struts.taglib.html.TOKEN", "");

            //请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.CookieContainer = cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.ProtocolVersion = httpVersion;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.Timeout = TimeOut;
            request.KeepAlive = false;
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(postData);
            writer.Flush();

            //回应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string htmlCommoditySummarization = reader.ReadToEnd();
            request.Abort();
            response.Close();
            if (string.IsNullOrEmpty(htmlCommoditySummarization))
            {
                return new List<CommoditySummarization>();
            }
            else
                return ParseHtmlCommoditySummarizations(tradeDate, htmlCommoditySummarization, statement, accountId);
        }

        /// <summary>
        /// 下载文件至"程序启动位置\statement\资金账号\结算单类型\yyyy-MM-dd.xls"，如果要下载的数据文件已存在，则跳过，不覆盖。
        /// </summary>
        /// <param name="tradeDate"></param>
        /// <param name="settlementType"></param>
        /// <returns></returns>
        private static string LoadExcelStatement(CookieContainer cookie, DateTime tradeDate, SettlementType settlementType, Guid accountId)
        {
            string directoryPath = _Session.DatabaseDirPath + "statement\\" + new StatementContext(typeof(Account)).Accounts.FirstOrDefault(acc => acc.Id == accountId).AccountNumber + "\\" + settlementType.ToString();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string filePath = string.Format(directoryPath + @"\{0}.xls", tradeDate.ToString("yyyy-MM-dd"));
            if (!File.Exists(filePath))
            {
                //请求
                string url = "https://investorservice.cfmmc.com/customer/setupViewCustomerDetailFromCompanyWithExcel.do";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;

                //回应
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();


                //文件流，流信息读到文件流中，读完关闭
                using (FileStream fs = File.Create(filePath))
                {
                    //建立字节组，并设置它的大小是多少字节
                    byte[] bytes = new byte[102400];
                    int n = 1;
                    while (n > 0)
                    {
                        //一次从流中读多少字节，并把值赋给Ｎ，当读完后，Ｎ为０,并退出循环
                        n = stream.Read(bytes, 0, 10240);
                        fs.Write(bytes, 0, n);　//将指定字节的流信息写入文件流中
                    }
                }

                stream.Dispose();
                stream.Close();
            }
            return filePath;
        }
#if true
        [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out   int ID);

        private static void ParseExcelStatement(StatementContext statement, string excelFilePath,
            out decimal? yesterdayBalance, out decimal? todayBalance, out decimal? remittanceTotal, out decimal? amount, Guid accountId)
        {
            //string tmpFilePath = excelFilePath;
            string tmpFilePath = excelFilePath + ".tmp";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = app.Workbooks.Open(excelFilePath, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            workBook.SaveCopyAs(tmpFilePath);
            workBook.Saved = true;
            app.UserControl = false;
            app.Quit();

            // 关闭EXCEL进程
            IntPtr ip = new IntPtr(app.Hwnd);
            int id = 0;
            GetWindowThreadProcessId(ip, out id);
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(id);
            p.Kill();

            //
            string connectString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=No;IMEX=1;'", tmpFilePath);
            OleDbConnection myConn = new OleDbConnection(connectString);//建立链接

            DataSet ds = new DataSet();
            new OleDbDataAdapter("Select * from [客户交易结算日报$]", myConn).Fill(ds, "客户交易结算日报");
            new OleDbDataAdapter("Select * from [品种汇总$]", myConn).Fill(ds, "品种汇总");
            new OleDbDataAdapter("Select * from [成交明细$]", myConn).Fill(ds, "成交明细");
            new OleDbDataAdapter("Select * from [平仓明细$]", myConn).Fill(ds, "平仓明细");
            new OleDbDataAdapter("Select * from [持仓明细$]", myConn).Fill(ds, "持仓明细");

            //
            var statementTable = ds.Tables["客户交易结算日报"];
            var commoditySummarizationsTable = ds.Tables["品种汇总"];
            var tradeDetailsTable = ds.Tables["成交明细"];
            var closedTradesTable = ds.Tables["平仓明细"];
            var positionDetailsTable = ds.Tables["持仓明细"];

            #region 客户交易结算日报

            //结单类型
            int currentRowIndex = 0;
            int? headerIndex = null, infoIndex = null, funStatusIndex = null, remittanceIndex = null, tradeIndex = null, positionIndex = null;
            while (currentRowIndex < statementTable.Rows.Count)
            {
                if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("客户交易结算日报"))
                {
                    headerIndex = currentRowIndex;
                }
                else if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("基本资料"))
                {
                    infoIndex = currentRowIndex;
                }
                else if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("资金状况"))
                {
                    funStatusIndex = currentRowIndex;
                }
                else if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("出入金明细"))
                {
                    remittanceIndex = currentRowIndex + 2;
                }
                else if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("成交汇总"))
                {
                    tradeIndex = currentRowIndex + 2;
                }
                else if (statementTable.Rows[currentRowIndex][0].ToString().Trim().Contains("持仓汇总"))
                {
                    positionIndex = currentRowIndex + 2;
                }
                currentRowIndex++;
            }

            SettlementType settlementType;
            if (statementTable.Rows[headerIndex.Value][0].ToString().Contains("逐日盯市"))
            {
                settlementType = SettlementType.date;
            }
            else if (statementTable.Rows[headerIndex.Value][0].ToString().Contains("逐笔对冲"))
            {
                settlementType = SettlementType.trade;
            }
            else
            {
                throw new Exception("程序判断结算类型的时候遇到错误，无法判断当前结单是逐日盯市还是逐笔对冲，请联系管理员！");
            }



            #region 基本资料
            //Account account = new Account();
            ////"客户内部资金账户" 
            ////string AccountNumber = statementTable.Rows[currentRowIndex + 1][2].ToString().Trim();
            //"交易日期" 
            DateTime tradeDate = Convert.ToDateTime(statementTable.Rows[infoIndex.Value + 1][7]);
            ////"客户名称" 42
            //account.CustomerName = statementTable.Rows[currentRowIndex + 2][2].ToString().Trim();
            ////"查询时间" 47
            ////"期货公司名称" 52
            //account.FuturesCompanyName = statementTable.Rows[currentRowIndex + 3][2].ToString().Trim();
            #endregion


            #region 资金状况
            FundStatus funStatus = new FundStatus();
            funStatus.Id = Guid.NewGuid();
            funStatus.AccountId = accountId;
            funStatus.Date = tradeDate;
            funStatus.SettlementType = settlementType;

            //"上日结存" 121
            yesterdayBalance = funStatus.YesterdayBalance = ToDecimal(statementTable.Rows[funStatusIndex.Value + 1][2].ToString());
            //"客户权益" 127
            funStatus.CustomerRights = ToDecimal(statementTable.Rows[funStatusIndex.Value + 1][7].ToString());
            //"当日存取合计" 132
            remittanceTotal = ToDecimal(statementTable.Rows[funStatusIndex.Value + 2][2].ToString());
            //"冻结资金"167
            funStatus.MatterDeposit = ToDecimal(statementTable.Rows[funStatusIndex.Value + 5][7].ToString());
            //"平仓盈亏" 14 2
            funStatus.ClosedProfit = ToDecimal(statementTable.Rows[funStatusIndex.Value + 3][2].ToString());
            //"保证金占用" 17 7
            funStatus.Margin = ToDecimal(statementTable.Rows[funStatusIndex.Value + 6][7].ToString());
            //"当日手续费"16 2
            funStatus.Commission = ToDecimal(statementTable.Rows[funStatusIndex.Value + 5][2].ToString());
            //"可用资金" 18 7
            funStatus.FreeMargin = ToDecimal(statementTable.Rows[funStatusIndex.Value + 7][7].ToString());
            //"当日结存" 17 2
            todayBalance = funStatus.TodayBalance = ToDecimal(statementTable.Rows[funStatusIndex.Value + 6][2].ToString());
            //"风险度" 19 7
            funStatus.VentureFactor = ToDouble(statementTable.Rows[funStatusIndex.Value + 8][7].ToString().Trim().Replace("%", ""));
            if (settlementType == SettlementType.trade)
            {
                //"浮动盈亏" 18 2
                funStatus.FloatingProfit = ToDecimal(statementTable.Rows[7][2].ToString());
            }
            //"追加保证金" 20 7
            funStatus.AdditionalMargin = ToDecimal(statementTable.Rows[funStatusIndex.Value + 9][7].ToString());

            //
            statement.AddFundStatus(funStatus);
            #endregion

            #region 出入金明细
            while (remittanceIndex.HasValue && !statementTable.Rows[remittanceIndex.Value][0].ToString().Trim().Equals("合计"))
            {
                Remittance rem = new Remittance();
                rem.Id = Guid.NewGuid();
                rem.AccountId = accountId;

                //"发生日期"0
                rem.Date = Convert.ToDateTime(statementTable.Rows[remittanceIndex.Value][0]);

                //"入金" 2
                rem.Deposit = ToDecimal(statementTable.Rows[remittanceIndex.Value][2].ToString());

                //"出金" 4
                rem.WithDrawal = ToDecimal(statementTable.Rows[remittanceIndex.Value][4].ToString());

                //"方式" 6
                rem.Type = statementTable.Rows[remittanceIndex.Value][6].ToString();

                //"摘要" 8
                rem.Summary = statementTable.Rows[remittanceIndex.Value][8].ToString();

                statement.AddRemittance(rem);
                remittanceIndex++;
            }
            #endregion

            #region 成交汇总
            //数据开始于：出入金合计行号+4
            amount = 0;
            while (tradeIndex.HasValue && !statementTable.Rows[tradeIndex.Value][0].ToString().Trim().Equals("合计"))
            {
                Trade trade = new Trade();
                trade.Id = Guid.NewGuid();
                trade.Date = tradeDate;
                trade.AccountId = accountId;

                //合约0
                trade.Item = statementTable.Rows[tradeIndex.Value][0].ToString();
                //买/卖1	
                trade.BS = statementTable.Rows[tradeIndex.Value][1].ToString();
                //投机/套保2
                trade.SH = statementTable.Rows[tradeIndex.Value][2].ToString();
                //成交价3
                trade.Price = ToDecimal(statementTable.Rows[tradeIndex.Value][3].ToString());
                //手数4	
                trade.Size = ToInt32(statementTable.Rows[tradeIndex.Value][4].ToString());
                //成交额5
                amount += trade.Amount = ToDecimal(statementTable.Rows[tradeIndex.Value][5].ToString());
                //开/平7	
                trade.OC = statementTable.Rows[tradeIndex.Value][7].ToString();
                //手续费8	
                trade.Commission = ToDecimal(statementTable.Rows[tradeIndex.Value][8].ToString());
                //平仓盈亏9
                trade.ClosedProfit = ToDecimal(statementTable.Rows[tradeIndex.Value][9].ToString());

                statement.AddTrade(trade);
                tradeIndex++;
            }
            #endregion

            #region 持仓汇总
            //数据开始于：成交汇总合计行号+4
            while (positionIndex.HasValue && !statementTable.Rows[positionIndex.Value][0].ToString().Trim().Equals("合计"))
            {
                Position position = new Position();
                position.Id = Guid.NewGuid();
                position.Date = tradeDate;
                position.SettlementType = settlementType;
                position.AccountId = accountId;

                //合约	0
                position.Item = statementTable.Rows[positionIndex.Value][0].ToString();
                //买持仓	1
                string strBuySize = statementTable.Rows[positionIndex.Value][1].ToString();
                if (!string.IsNullOrEmpty(strBuySize))
                    position.BuySize = ToInt32(strBuySize);
                //买均价	2
                string strBuyAveragePrice = statementTable.Rows[positionIndex.Value][2].ToString();
                if (!string.IsNullOrEmpty(strBuyAveragePrice))
                    position.BuyAveragePrice = ToDecimal(strBuyAveragePrice);
                //卖持仓	3
                string strSaleSize = statementTable.Rows[positionIndex.Value][3].ToString();
                if (!string.IsNullOrEmpty(strSaleSize))
                    position.SaleSize = ToInt32(strSaleSize);
                //卖均价	4
                string strSaleAveragePrice = statementTable.Rows[positionIndex.Value][4].ToString();
                if (!string.IsNullOrEmpty(strSaleAveragePrice))
                    position.SaleAveragePrice = ToDecimal(strSaleAveragePrice);
                //昨结算价5	
                position.YesterdaySettlementPrice = ToDecimal(statementTable.Rows[positionIndex.Value][5].ToString());
                //今结算价6	
                position.TodaySettlementPrice = ToDecimal(statementTable.Rows[positionIndex.Value][6].ToString());
                //浮动盈亏7	
                position.Profit = ToDecimal(statementTable.Rows[positionIndex.Value][7].ToString());
                //交易保证金8	
                position.Margin = ToDecimal(statementTable.Rows[positionIndex.Value][8].ToString());
                //投机/套保9
                position.SH = statementTable.Rows[positionIndex.Value][9].ToString();

                //
                statement.AddPosition(position);
                positionIndex++;
            }
            #endregion

            #endregion

            #region 品种汇总

            //结算方式ROWS[5][0]
            //交易日期1,7
            //while (!statementTable.Rows[currentRowIndex][0].ToString().Trim().Equals("基本资料"))
            //{
            //    currentRowIndex++;
            //}
            //tradeDate = Convert.ToDateTime(commoditySummarizationsTable.Rows[currentRowIndex + 1][7]);

            //数据开始于：9
            currentRowIndex = 0;
            while (!commoditySummarizationsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("品种汇总"))
            {
                currentRowIndex++;
            }
            currentRowIndex += 2;
            while (!commoditySummarizationsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("合计"))
            {
                CommoditySummarization commoditySummarization = new CommoditySummarization();
                commoditySummarization.Id = Guid.NewGuid();
                commoditySummarization.Date = tradeDate;
                commoditySummarization.AccountId = accountId;

                //品种0	
                commoditySummarization.Commodity = commoditySummarizationsTable.Rows[currentRowIndex][0].ToString();
                //手数1	
                commoditySummarization.Size = ToInt32(commoditySummarizationsTable.Rows[currentRowIndex][1].ToString());
                //成交额2		
                commoditySummarization.Amount = ToDecimal(commoditySummarizationsTable.Rows[currentRowIndex][2].ToString());
                //手续费4	
                commoditySummarization.Commission = ToDecimal(commoditySummarizationsTable.Rows[currentRowIndex][4].ToString());
                //平仓盈亏5
                commoditySummarization.ClosedProfit = ToDecimal(commoditySummarizationsTable.Rows[currentRowIndex][5].ToString());

                statement.AddCommoditySummarization(commoditySummarization);
                currentRowIndex++;
            }

            #endregion

            #region 成交明细

            //结算方式ROWS[5][0]
            //交易日期1,7

            //数据开始于：9
            currentRowIndex = 0;
            while (!tradeDetailsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("成交明细"))
            {
                currentRowIndex++;
            }
            currentRowIndex += 2;
            while (!tradeDetailsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("合计"))
            {
                TradeDetail tradeDetail = new TradeDetail();
                tradeDetail.Id = Guid.NewGuid();
                tradeDetail.AccountId = accountId;

                //合约0	
                tradeDetail.Item = tradeDetailsTable.Rows[currentRowIndex][0].ToString();
                //成交序号1	
                tradeDetail.Ticket = tradeDetailsTable.Rows[currentRowIndex][1].ToString();
                //买/卖3	
                tradeDetail.BS = tradeDetailsTable.Rows[currentRowIndex][3].ToString();
                //投机/套保4	
                tradeDetail.SH = tradeDetailsTable.Rows[currentRowIndex][4].ToString();
                //成交价5	
                tradeDetail.Price = ToDecimal(tradeDetailsTable.Rows[currentRowIndex][5].ToString());
                //手数6	
                tradeDetail.Size = ToInt32(tradeDetailsTable.Rows[currentRowIndex][6].ToString());
                //成交额7	
                tradeDetail.Amount = ToDecimal(tradeDetailsTable.Rows[currentRowIndex][7].ToString());
                //开/平8	
                tradeDetail.OC = tradeDetailsTable.Rows[currentRowIndex][8].ToString();
                //手续费9	
                tradeDetail.Commission = ToDecimal(tradeDetailsTable.Rows[currentRowIndex][9].ToString());
                //平仓盈亏10	
                string strClosedProfit = tradeDetailsTable.Rows[currentRowIndex][10].ToString();
                if (!strClosedProfit.Equals("--"))
                    tradeDetail.ClosedProfit = ToDecimal(strClosedProfit);
                //实际成交日期11  成交时间2	
                tradeDetail.ActualTime = Convert.ToDateTime(string.Format("{0} {1}", tradeDetailsTable.Rows[currentRowIndex][11].ToString(), tradeDetailsTable.Rows[currentRowIndex][2].ToString()));

                statement.AddTradeDetail(tradeDetail);
                currentRowIndex++;
            }

            #endregion

            #region 平仓明细

            //结算方式ROWS[5][0]
            //交易日期1,7

            //数据开始于：9
            currentRowIndex = 0;
            while (!closedTradesTable.Rows[currentRowIndex][0].ToString().Trim().Equals("平仓明细"))
            {
                currentRowIndex++;
            }
            currentRowIndex += 2;
            while (!closedTradesTable.Rows[currentRowIndex][0].ToString().Trim().Equals("合计"))
            {
                ClosedTradeDetail closedTradeDetail = new ClosedTradeDetail();
                closedTradeDetail.Id = Guid.NewGuid();
                closedTradeDetail.ActualDate = tradeDate;
                closedTradeDetail.AccountId = accountId;

                //合约0	
                closedTradeDetail.Item = closedTradesTable.Rows[currentRowIndex][0].ToString();
                //成交序号1	
                closedTradeDetail.TicketForClose = closedTradesTable.Rows[currentRowIndex][1].ToString();
                //买/卖2	
                closedTradeDetail.BS = closedTradesTable.Rows[currentRowIndex][2].ToString();
                //成交价3	
                closedTradeDetail.PriceForClose = ToDecimal(closedTradesTable.Rows[currentRowIndex][3].ToString());
                //开仓价4	
                closedTradeDetail.PriceForOpen = ToDecimal(closedTradesTable.Rows[currentRowIndex][4].ToString());
                //手数5	
                closedTradeDetail.Size = ToInt32(closedTradesTable.Rows[currentRowIndex][5].ToString());
                //昨结算价6	
                closedTradeDetail.YesterdaySettlementPrice = ToDecimal(closedTradesTable.Rows[currentRowIndex][6].ToString());
                //平仓盈亏7	
                closedTradeDetail.ClosedProfit = ToDecimal(closedTradesTable.Rows[currentRowIndex][7].ToString());
                //原成交序号8	
                closedTradeDetail.TicketForOpen = closedTradesTable.Rows[currentRowIndex][8].ToString();
                //实际成交日期9
                //closedTradeDetail.ActualDate = Convert.ToDateTime(closedTradesTable.Rows[currentRowIndex][9].ToString());

                statement.AddClosedTradeDetail(closedTradeDetail);
                currentRowIndex++;
            }

            #endregion

            #region 持仓明细

            //结算方式ROWS[5][0]

            //数据开始于：9
            currentRowIndex = 0;
            while (!positionDetailsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("持仓明细"))
            {
                currentRowIndex++;
            }
            currentRowIndex += 2;
            List<PositionDetail> positionDetails = new List<PositionDetail>();
            while (!positionDetailsTable.Rows[currentRowIndex][0].ToString().Trim().Equals("合计"))
            {
                PositionDetail positionDetail = new PositionDetail();
                positionDetail.Id = Guid.NewGuid();
                positionDetail.DateForPosition = tradeDate;
                positionDetail.SettlementType = settlementType;
                positionDetail.AccountId = accountId;

                //合约0	
                positionDetail.Item = positionDetailsTable.Rows[currentRowIndex][0].ToString();
                //成交序号1	
                positionDetail.Ticket = positionDetailsTable.Rows[currentRowIndex][1].ToString();
                //买持仓2	
                string strBuySize = positionDetailsTable.Rows[currentRowIndex][2].ToString();
                if (!string.IsNullOrEmpty(strBuySize))
                    positionDetail.BuySize = ToInt32(strBuySize);
                //买入价3	
                positionDetail.BuyPrice = ToDecimal(positionDetailsTable.Rows[currentRowIndex][3].ToString());
                //卖持仓4	
                positionDetail.SaleSize = ToInt32(positionDetailsTable.Rows[currentRowIndex][4].ToString());
                //卖出价5	
                positionDetail.SalePrice = ToDecimal(positionDetailsTable.Rows[currentRowIndex][5].ToString());
                //昨结算价6	
                positionDetail.YesterdaySettlementPrice = ToDecimal(positionDetailsTable.Rows[currentRowIndex][6].ToString());
                //今结算价7	
                positionDetail.TodaySettlementPrice = ToDecimal(positionDetailsTable.Rows[currentRowIndex][7].ToString());
                //浮动盈亏8	
                positionDetail.Profit = ToDecimal(positionDetailsTable.Rows[currentRowIndex][8].ToString());
                //投机/套保9
                positionDetail.SH = positionDetailsTable.Rows[currentRowIndex][9].ToString();
                //交易编码10	
                positionDetail.TradeCode = positionDetailsTable.Rows[currentRowIndex][10].ToString();
                //实际成交日期11
                positionDetail.DateForActual = Convert.ToDateTime(positionDetailsTable.Rows[currentRowIndex][11].ToString());

                statement.AddPositionDetail(positionDetail);
                currentRowIndex++;
            }

            #endregion

            File.Delete(tmpFilePath);

        }

        private static decimal ToDecimal(String value)
        {
            if (String.IsNullOrEmpty(value) || value.Contains("--"))
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(value);
            }
        }
        private static double ToDouble(String value)
        {
            if (String.IsNullOrEmpty(value) || value.Contains("-"))
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(value);
            }
        }
        private static int ToInt32(String value)
        {
            if (String.IsNullOrEmpty(value) || value.Contains("-"))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        private static int tryCount = 0;
        public static void GetStatementByLoadExcel(CookieContainer cookie, DateTime tradeDate, SettlementType settlementType, StatementContext statement,
            out decimal? yesterdayBalance, out decimal? todayBalance, out decimal? remittance, out decimal? amount, Guid accountId)
        {
            try
            {
                /// 访问主页信息
                #region
                //设置参数
                string url = "https://investorservice.cfmmc.com/customer/setParameter.do";
                string postData = string.Format("tradeDate={0}&byType={1}", tradeDate.ToString("yyyy-MM-dd"), settlementType.ToString());

                //请求
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write(postData);
                writer.Flush();

                //回应
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string htmlStatementHomePage = reader.ReadToEnd();
                request.Abort();
                response.Close();
                #endregion

                if (htmlStatementHomePage.IndexOf("查询系统目前不提供从今日起") == -1 && htmlStatementHomePage.IndexOf("系统中无投资者") == -1 && htmlStatementHomePage.IndexOf("为非交易日，请重新选择交易日期") == -1)
                {
                    string excelFilePath = LoadExcelStatement(cookie, tradeDate, settlementType, accountId);
                    ParseExcelStatement(statement, excelFilePath, out yesterdayBalance, out todayBalance, out remittance, out amount, accountId);
                }
                else
                {
                    yesterdayBalance = todayBalance = remittance = amount = null;
                }
                //
                tryCount = 0;
            }
            catch (IOException)
            {
                if (tryCount <= 10)
                {
                    GetStatementByLoadExcel(cookie, tradeDate, settlementType, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, accountId);
                    tryCount++;
                }
                else
                {
                    MessageBox.Show("网络IO读取多次失败，请运行更新程序。");
                    throw;
                }
            }
            catch (WebException)
            {
                if (tryCount <= 10)
                {
                    GetStatementByLoadExcel(cookie, tradeDate, settlementType, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, accountId);
                    tryCount++;
                }
                else
                {
                    MessageBox.Show("连接监控中心时因网络问题多次中断，请保证网络畅通后，再次运行更新程序。");
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Helper GetStatementByLoadExcel() throw " + ex.Message);
                //throw;
                yesterdayBalance = todayBalance = remittance = amount = null;
            }
        }

#endif
    }
}
