using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FuturesAssistantWPF.Models
{
    public class StatementStream
    {
        private string connectionStr = @"Data Source=" + _Session.DatabaseFilePath;
        public StatementStream()
        {
            if (!Directory.Exists(_Session.DatabaseDirPath))
                Directory.CreateDirectory(_Session.DatabaseDirPath);

            if (!File.Exists(_Session.DatabaseFilePath))
            {
                SQLiteConnection conn = null;
                try
                {
                    conn = new SQLiteConnection(connectionStr);//创建数据库实例，指定文件位置  
                    conn.Open();//打开数据库，若文件不存在会自动创建  

                    // 创建Account表
                    string sql =
                        "CREATE TABLE IF NOT EXISTS Account("
                    + "Id TEXT PRIMARY KEY,"
                    + "Type INTEGER,"
                    + "IsAllowLoad BOOL,"
                    + "AccountNumber TEXT,"
                    + "Password TEXT,"
                    + "CustomerName TEXT,"
                    + "FuturesCompanyName TEXT,"
                    + "UserId TEXT)";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // 创建ClosedTradeDetail表
                    sql =
                        "CREATE TABLE IF NOT EXISTS ClosedTradeDetail("
                    + "Id TEXT PRIMARY KEY,"
                    + "ActualDate DATETIME,"
                    + "Item TEXT,"
                    + "TicketForClose TEXT,"
                    + "BS TEXT,"
                    + "PriceForClose REAL,"
                    + "PriceForOpen REAL,"
                    + "Size INTEGER,"
                    + "YesterdaySettlementPrice REAL,"
                    + "ClosedProfit REAL,"
                    + "TicketForOpen TEXT,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建Commodity表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Commodity("
                    + "Code TEXT PRIMARY KEY,"
                    + "Name TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // 创建CommoditySummarization表
                    sql =
                        "CREATE TABLE IF NOT EXISTS CommoditySummarization("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "Commodity TEXT,"
                    + "Size INTEGER,"
                    + "Amount REAL,"
                    + "Commission REAL,"
                    + "ClosedProfit REAL,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建FundStatus表
                    sql =
                        "CREATE TABLE IF NOT EXISTS FundStatus("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "YesterdayBalance REAL,"
                    + "CustomerRights REAL,"
                    + "Remittance REAL,"
                    + "MatterDeposit REAL,"
                    + "ClosedProfit REAL,"
                    + "Margin REAL,"
                    + "Commission REAL,"
                    + "FreeMargin REAL,"
                    + "TodayBalance REAL,"
                    + "VentureFactor REAL,"
                    + "FloatingProfit REAL,"
                    + "AdditionalMargin REAL,"
                    + "SettlementType INTEGER,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建Position表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Position("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "Item TEXT,"
                    + "BuySize INTEGER,"
                    + "BuyAveragePrice REAL,"
                    + "SaleSize INTEGER,"
                    + "SaleAveragePrice REAL,"
                    + "YesterdaySettlementPrice REAL,"
                    + "TodaySettlementPrice REAL,"
                    + "Profit REAL,"
                    + "Margin REAL,"
                    + "SH TEXT,"
                    + "SettlementType INTEGER,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建PositionDetail表
                    sql =
                        "CREATE TABLE IF NOT EXISTS PositionDetail("
                    + "Id TEXT PRIMARY KEY,"
                    + "DateForPosition DATETIME,"
                    + "DateForActual DATETIME,"
                    + "Item TEXT,"
                    + "Ticket TEXT,"
                    + "BuySize INTEGER,"
                    + "BuyPrice REAL,"
                    + "SaleSize INTEGER,"
                    + "SalePrice REAL,"
                    + "YesterdaySettlementPrice REAL,"
                    + "TodaySettlementPrice REAL,"
                    + "Profit REAL,"
                    + "SH TEXT,"
                    + "TradeCode TEXT,"
                    + "SettlementType INTEGER,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建Remittance表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Remittance("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "Deposit REAL,"
                    + "WithDrawal REAL,"
                    + "Type TEXT,"
                    + "Summary TEXT,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();



                    // 创建Trade表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Trade("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "Item TEXT,"
                    + "BS TEXT,"
                    + "OC TEXT,"
                    + "Price REAL,"
                    + "Size INTEGER,"
                    + "Amount REAL,"
                    + "Commission REAL,"
                    + "SH TEXT,"
                    + "ClosedProfit REAL,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建TradeDetail表
                    sql =
                        "CREATE TABLE IF NOT EXISTS TradeDetail("
                    + "Id TEXT PRIMARY KEY,"
                    + "ActualTime DATETIME,"
                    + "Item TEXT,"
                    + "Ticket TEXT,"
                    + "BS TEXT,"
                    + "SH TEXT,"
                    + "Price REAL,"
                    + "Size INTEGER,"
                    + "Amount REAL,"
                    + "OC TEXT,"
                    + "Commission REAL,"
                    + "ClosedProfit REAL,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建User表
                    sql =
                        "CREATE TABLE IF NOT EXISTS User("
                    + "Id TEXT PRIMARY KEY,"
                    + "UserName TEXT,"
                    + "UserPassword TEXT,"
                    + "Email TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建Stock表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Stock("
                    + "Id TEXT PRIMARY KEY,"
                    + "Date DATETIME,"
                    + "Open REAL,"
                    + "High REAL,"
                    + "Low REAL,"
                    + "Close REAL,"
                    + "Volume REAL,"
                    + "AccountId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    // 创建Parameter表
                    sql =
                        "CREATE TABLE IF NOT EXISTS Parameter("
                    + "Id TEXT PRIMARY KEY,"
                    + "Name TEXT,"
                    + "Value TEXT,"
                    + "UserId TEXT)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    conn.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (conn != null)
                        conn.Close();
                }
            }
        }

        public List<Commodity> GetCommoditys()
        {
            List<Commodity> result = new List<Commodity>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Commodity");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Commodity model = new Commodity();
                    model.Code = reader.GetString(0);
                    model.Name = reader.GetString(1);
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<Account> GetAccounts()
        {
            List<Account> result = new List<Account>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Account");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Account model = new Account();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Type = reader.GetInt32(1);
                    model.IsAllowLoad = reader.GetBoolean(2);
                    model.AccountNumber = reader.GetString(3);
                    model.Password = reader.GetString(4);
                    model.CustomerName = reader.GetString(5);
                    model.FuturesCompanyName = reader.GetString(6);
                    model.UserId = Guid.Parse(reader.GetString(7));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<ClosedTradeDetail> GetClosedTradeDetails()
        {
            List<ClosedTradeDetail> result = new List<ClosedTradeDetail>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM ClosedTradeDetail");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ClosedTradeDetail model = new ClosedTradeDetail();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.ActualDate = reader.GetDateTime(1);
                    model.Item = reader.GetString(2);
                    model.TicketForClose = reader.GetString(3);
                    model.BS = reader.GetString(4);
                    model.PriceForClose = reader.GetDecimal(5);
                    model.PriceForOpen = reader.GetDecimal(6);
                    model.Size = reader.GetInt32(7);
                    model.YesterdaySettlementPrice = reader.GetDecimal(8);
                    model.ClosedProfit = reader.GetDecimal(9);
                    model.TicketForOpen = reader.GetString(10);
                    model.AccountId = Guid.Parse(reader.GetString(11));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<CommoditySummarization> GetCommoditySummarizations()
        {
            List<CommoditySummarization> result = new List<CommoditySummarization>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM CommoditySummarization");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CommoditySummarization model = new CommoditySummarization();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.Commodity = reader.GetString(2);
                    model.Size = reader.GetInt32(3);
                    model.Amount = reader.GetDecimal(4);
                    model.Commission = reader.GetDecimal(5);
                    model.ClosedProfit = reader.GetDecimal(6);
                    model.AccountId = Guid.Parse(reader.GetString(7));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<FundStatus> GetFundStatus()
        {
            List<FundStatus> result = new List<FundStatus>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM FundStatus");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    FundStatus model = new FundStatus();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.YesterdayBalance = reader.GetDecimal(2);
                    model.CustomerRights = reader.GetDecimal(3);
                    model.Remittance = reader.GetDecimal(4);
                    model.MatterDeposit = reader.GetDecimal(5);
                    model.ClosedProfit = reader.GetDecimal(6);
                    model.Margin = reader.GetDecimal(7);
                    model.Commission = reader.GetDecimal(8);
                    model.FreeMargin = reader.GetDecimal(9);
                    model.TodayBalance = reader.GetDecimal(10);
                    model.VentureFactor = reader.GetDouble(11);
                    model.FloatingProfit = reader.GetDecimal(12);
                    model.AdditionalMargin = reader.GetDecimal(13);
                    model.SettlementType = (SettlementType)reader.GetInt32(14);
                    model.AccountId = Guid.Parse(reader.GetString(15));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<Position> GetPositions()
        {
            List<Position> result = new List<Position>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Position");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Position model = new Position();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.Item = reader.GetString(2);
                    model.BuySize = reader.GetInt32(3);
                    model.BuyAveragePrice = reader.GetDecimal(4);
                    model.SaleSize = reader.GetInt32(5);
                    model.SaleAveragePrice = reader.GetDecimal(6);
                    model.YesterdaySettlementPrice = reader.GetDecimal(7);
                    model.TodaySettlementPrice = reader.GetDecimal(8);
                    model.Profit = reader.GetDecimal(9);
                    model.Margin = reader.GetDecimal(10);
                    model.SH = reader.GetString(11);
                    model.SettlementType = (SettlementType)reader.GetInt32(12);
                    model.AccountId = Guid.Parse(reader.GetString(13));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<PositionDetail> GetPositionDetails()
        {
            List<PositionDetail> result = new List<PositionDetail>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM PositionDetail");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PositionDetail model = new PositionDetail();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.DateForPosition = reader.GetDateTime(1);
                    model.DateForActual = reader.GetDateTime(2);
                    model.Item = reader.GetString(3);
                    model.Ticket = reader.GetString(4);
                    model.BuySize = reader.GetInt32(5);
                    model.BuyPrice = reader.GetDecimal(6);
                    model.SaleSize = reader.GetInt32(7);
                    model.SalePrice = reader.GetDecimal(8);
                    model.YesterdaySettlementPrice = reader.GetDecimal(9);
                    model.TodaySettlementPrice = reader.GetDecimal(10);
                    model.Profit = reader.GetDecimal(11);
                    model.SH = reader.GetString(12);
                    model.TradeCode = reader.GetString(13);
                    model.SettlementType = (SettlementType)reader.GetInt32(14);
                    model.AccountId = Guid.Parse(reader.GetString(15));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<Remittance> GetRemittances()
        {
            List<Remittance> result = new List<Remittance>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Remittance");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Remittance model = new Remittance();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.Deposit = reader.GetDecimal(2);
                    model.WithDrawal = reader.GetDecimal(3);
                    model.Type = reader.GetString(4);
                    model.Summary = reader.GetString(5);
                    model.AccountId = Guid.Parse(reader.GetString(6));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<Parameter> GetParameters()
        {
            List<Parameter> result = new List<Parameter>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Parameter");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Parameter model = new Parameter();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Name = reader.GetString(1);
                    model.Value = reader.GetString(2);
                    model.UserId = Guid.Parse(reader.GetString(3));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public void Edit(Parameter model)
        {
            SQLiteConnection conn = null;
            SQLiteTransaction tran = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);//创建数据库实例，指定文件位置  
                conn.Open();//打开数据库，若文件不存在会自动创建  
                tran = conn.BeginTransaction();
                string sql =
                    "UPDATE Parameter " +
                    "SET Name=@Name,Value=@Value,UserId=@UserId " +
                    "WHERE Id=@Id";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Transaction = tran;
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
                cmd.Parameters.AddWithValue("@Value", model.Value);
                cmd.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public List<Stock> GetStocks()
        {
            List<Stock> result = new List<Stock>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Stock");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Stock model = new Stock();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.Open = reader.GetDecimal(2);
                    model.High = reader.GetDecimal(3);
                    model.Low = reader.GetDecimal(4);
                    model.Close = reader.GetDecimal(5);
                    model.Volume = reader.GetDecimal(6);
                    model.AccountId = Guid.Parse(reader.GetString(7));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<Trade> GetTrades()
        {
            List<Trade> result = new List<Trade>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM Trade");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Trade model = new Trade();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.Date = reader.GetDateTime(1);
                    model.Item = reader.GetString(2);
                    model.BS = reader.GetString(3);
                    model.OC = reader.GetString(4);
                    model.Price = reader.GetDecimal(5);
                    model.Size = reader.GetInt32(6);
                    model.Amount = reader.GetDecimal(7);
                    model.Commission = reader.GetDecimal(8);
                    model.SH = reader.GetString(9);
                    model.ClosedProfit = reader.GetDecimal(10);
                    model.AccountId = Guid.Parse(reader.GetString(11));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<TradeDetail> GetTradeDetails()
        {
            List<TradeDetail> result = new List<TradeDetail>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM TradeDetail");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TradeDetail model = new TradeDetail();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.ActualTime = reader.GetDateTime(1);
                    model.Item = reader.GetString(2);
                    model.Ticket = reader.GetString(3);
                    model.BS = reader.GetString(4);
                    model.SH = reader.GetString(5);
                    model.Price = reader.GetDecimal(6);
                    model.Size = reader.GetInt32(7);
                    model.Amount = reader.GetDecimal(8);
                    model.OC = reader.GetString(9);
                    model.Commission = reader.GetDecimal(10);
                    model.ClosedProfit = reader.GetDecimal(11);
                    model.AccountId = Guid.Parse(reader.GetString(12));
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
        public List<User> GetUsers()
        {
            List<User> result = new List<User>();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);
                conn.Open();
                string sql = string.Format("SELECT * FROM User");
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User model = new User();
                    model.Id = Guid.Parse(reader.GetString(0));
                    model.UserName = reader.GetString(1);
                    model.UserPassword = reader.GetString(2);
                    model.Email = reader.GetString(3);
                    result.Add(model);
                }
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }

    }
}
