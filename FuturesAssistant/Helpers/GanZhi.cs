using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace FuturesAssistant.Helpers
{
    public class GanZhi
    {
        private BinaryFormatter formatter = new BinaryFormatter();
        private IDictionary<DateTime, SolarTerm> SolarTermDictionary = new Dictionary<DateTime, SolarTerm>();
        private string[] TianGan = { "癸", "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private string[] DiZhi = { "亥", "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private string[] ChineseMonth = { "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥", "子", "丑" };
        public string TianGanYear { get; set; }
        public string DiZhiYear { get; set; }
        public string TianGanMonth { get; set; }
        public string DiZhiMonth { get; set; }
        public string TianGanDay { get; set; }
        public string DiZhiDay { get; set; }
        public string TianGanHour { get; set; }
        public string DiZhiHour { get; set; }

        public GanZhi(DateTime dateTime, IDictionary<DateTime, SolarTerm> solarTermDictionary)
        {
            try
            {
                SolarTermDictionary = solarTermDictionary;
                ConvertToGanZhi(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ConvertToGanZhi(DateTime dateTime)
        {
            try
            {
                if (dateTime < new DateTime(1800, 1, 1, 0, 0, 0) || dateTime > new DateTime(2100, 12, 31, 23, 59, 59))
                {
                    throw new DateTimeOutOfRangeException("超出时间允许范围【1800年1月1日-2100年12月31日】！");
                }


                ChineseLunisolarCalendar chineseLunisolarCalendar = new ChineseLunisolarCalendar();

                // 年干支
                int sexagenaryYear = chineseLunisolarCalendar.GetSexagenaryYear(new DateTime(dateTime.Year, 6, 6));
                var liChunDay = SolarTermDictionary.FirstOrDefault(m => m.Key.Year == dateTime.Year && m.Value == SolarTerm.立春);
                if (dateTime < liChunDay.Key.Date)
                {
                    sexagenaryYear -= 1;
                }
                string tianGanYear = TianGan[chineseLunisolarCalendar.GetCelestialStem(sexagenaryYear)];
                string diZhiYear = DiZhi[chineseLunisolarCalendar.GetTerrestrialBranch(sexagenaryYear)];

                // 月干支
                var nextSolarTerm = SolarTermDictionary.Where(m => m.Key.Year == dateTime.Year && m.Key.Date > dateTime.Date).OrderBy(m => m.Key);
                int chineseMonth = 0;
                if (nextSolarTerm.Count() > 0)
                {
                    chineseMonth = ((int)nextSolarTerm.FirstOrDefault().Value - 1) / 2 + 1;
                }
                else
                {
                    chineseMonth = 11;
                }
                string tianGanMonth = GetTianGanMonth(tianGanYear, chineseMonth);
                string diZhiMonth = ChineseMonth[chineseMonth];

                // 日干支  1800年1月1日00:00  庚寅日 丙子时
                var timeSpan = dateTime - (new DateTime(1800, 1, 1, 0, 0, 0).AddHours(-1));
                string tianGanDay = TianGan[((int)(timeSpan.TotalDays % 10) + 7) % 10];
                string diZhiDay = DiZhi[((int)(timeSpan.TotalDays % 12) + 3) % 12];

                // 时干支 
                string tianGanHour = TianGan[((int)(timeSpan.TotalHours / 2 % 10) + 3) % 10];
                string diZhiHour = DiZhi[((int)(timeSpan.TotalHours / 2 % 12) + 1) % 12];

                TianGanYear = tianGanYear;
                TianGanMonth = tianGanMonth;
                TianGanDay = tianGanDay;
                TianGanHour = tianGanHour;
                DiZhiYear = diZhiYear;
                DiZhiMonth = diZhiMonth;
                DiZhiDay = diZhiDay;
                DiZhiHour = diZhiHour;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetTianGanMonth(string tianGanYear, int chineseMonth)
        {
            // 甲己之年丙作首
            if (tianGanYear.Equals("甲") || tianGanYear.Equals("己"))
            {
                return TianGan[(2 + chineseMonth) % 10];
            }
            //乙庚之岁戊为头
            else if (tianGanYear.Equals("乙") || tianGanYear.Equals("庚"))
            {
                return TianGan[(4 + chineseMonth) % 10];
            }
            //丙辛必定寻庚起
            else if (tianGanYear.Equals("丙") || tianGanYear.Equals("辛"))
            {
                return TianGan[(6 + chineseMonth) % 10];
            }
            //丁壬壬位顺行流
            else if (tianGanYear.Equals("丁") || tianGanYear.Equals("壬"))
            {
                return TianGan[(8 + chineseMonth) % 10];
            }
            //若问戊癸何方发 甲寅之上好追求
            else if (tianGanYear.Equals("戊") || tianGanYear.Equals("癸"))
            {
                return TianGan[(chineseMonth) % 10];
            }
            return null;
        }

        public IDictionary<DateTime, SolarTerm> GetSolarTermDictionary(int startYear, int endYear)
        {
            CookieContainer cookie = new CookieContainer();
            IDictionary<DateTime, SolarTerm> solarTermDictionary = new Dictionary<DateTime, SolarTerm>();

            for (int year = startYear; year <= endYear; year++)
            {
                string url = "http://www.sheup.com/24jieqi.php";
                string postData = string.Format("jieqi={0}", year);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.CookieContainer = cookie;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.Timeout = 5000;
                StreamWriter sw = new StreamWriter(request.GetRequestStream());
                sw.Write(postData);
                sw.Flush();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("gb2312"));
                string result = sr.ReadToEnd();
                sw.Close();
                sr.Close();
                s.Close();


                result = result.Substring(result.IndexOf("<li><strong>小寒</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                string dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.小寒);

                result = result.Substring(result.IndexOf("<li><strong>大寒</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.大寒);

                result = result.Substring(result.IndexOf("<li><strong>立春</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.立春);

                result = result.Substring(result.IndexOf("<li><strong>雨水</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.雨水);

                result = result.Substring(result.IndexOf("<li><strong>惊蛰</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.惊蛰);

                result = result.Substring(result.IndexOf("<li><strong>春分</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.春分);

                result = result.Substring(result.IndexOf("<li><strong>清明</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.清明);

                result = result.Substring(result.IndexOf("<li><strong>谷雨</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.谷雨);

                result = result.Substring(result.IndexOf("<li><strong>立夏</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.立夏);

                result = result.Substring(result.IndexOf("<li><strong>小满</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.小满);

                result = result.Substring(result.IndexOf("<li><strong>芒种</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.芒种);

                result = result.Substring(result.IndexOf("<li><strong>夏至</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.夏至);

                result = result.Substring(result.IndexOf("<li><strong>小暑</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.小暑);

                result = result.Substring(result.IndexOf("<li><strong>大暑</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.大暑);

                result = result.Substring(result.IndexOf("<li><strong>立秋</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.立秋);

                result = result.Substring(result.IndexOf("<li><strong>处暑</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.处暑);

                result = result.Substring(result.IndexOf("<li><strong>白露</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.白露);

                result = result.Substring(result.IndexOf("<li><strong>秋分</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.秋分);

                result = result.Substring(result.IndexOf("<li><strong>寒露</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.寒露);

                result = result.Substring(result.IndexOf("<li><strong>霜降</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.霜降);

                result = result.Substring(result.IndexOf("<li><strong>立冬</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.立冬);

                result = result.Substring(result.IndexOf("<li><strong>小雪</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.小雪);

                result = result.Substring(result.IndexOf("<li><strong>大雪</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.大雪);

                result = result.Substring(result.IndexOf("<li><strong>冬至</strong>"));
                result = result.Substring(result.IndexOf(")") + 1);
                dateTime = result.Substring(0, result.IndexOf("<"));
                solarTermDictionary.Add(ParseDate(year, dateTime), SolarTerm.冬至);
            }
            return solarTermDictionary;
        }

        private DateTime ParseDate(int year, string dateTime)
        {
            // 01月06日 05:36
            string month = dateTime.Substring(0, dateTime.IndexOf("月"));
            dateTime = dateTime.Substring(dateTime.IndexOf("月") + 1);
            string day = dateTime.Substring(0, dateTime.IndexOf("日"));
            dateTime = dateTime.Substring(dateTime.IndexOf("日") + 1);
            string hour = dateTime.Substring(0, dateTime.IndexOf(":"));
            string minite = dateTime.Substring(dateTime.IndexOf(":") + 1);
            return new DateTime(year, Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minite), 0);
        }

    }
    public class DateTimeOutOfRangeException : Exception
    {
        public DateTimeOutOfRangeException()
            : base("超出时间允许范围【1800年1月1日-2100年12月31日】！")
        { }
        public DateTimeOutOfRangeException(string message)
            : base(message)
        { }
    }
    public enum SolarTerm
    {
        立春 = 24, 雨水 = 1,
        惊蛰 = 2, 春分 = 3,
        清明 = 4, 谷雨 = 5,
        立夏 = 6, 小满 = 7,
        芒种 = 8, 夏至 = 9,
        小暑 = 10, 大暑 = 11,
        立秋 = 12, 处暑 = 13,
        白露 = 14, 秋分 = 15,
        寒露 = 16, 霜降 = 17,
        立冬 = 18, 小雪 = 19,
        大雪 = 20, 冬至 = 21,
        小寒 = 22, 大寒 = 23
    }

   
}
