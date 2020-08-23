using FuturesAssistantWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Models
{
    public class StatementContext : IDisposable
    {
        private string connectionStr = @"Data Source=" + _Session.DatabaseFilePath;

        List<SQLiteCommand> cmds = new List<SQLiteCommand>();

        #region Commodity
        public List<Commodity> Commoditys { get; set; }
        public void AddCommodity(Commodity model)
        {
            Commoditys.Add(model);

            string sql = "INSERT INTO Commodity (Code,Name) VALUES(@Code,@Name)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmds.Add(cmd);
        }
        public void AddCommoditys(List<Commodity> models)
        {
            Commoditys.AddRange(models);

            foreach (var model in models)
            {
                string sql = "INSERT INTO Commodity (Code,Name) VALUES(@Code,@Name)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@Code", model.Code);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmds.Add(cmd);
            }
        }
        public void EditCommodity(Commodity model)
        {
            string sql = "UPDATE Commodity " +
                "SET Name=@Name " +
                "WHERE Code=@Code";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmds.Add(cmd);
        }
        #endregion


        #region User
        public List<User> Users { get; set; }
        public void AddUser(User model)
        {
            Users.Add(model);

            string sql = "INSERT INTO User (Id,UserName,UserPassword,Email) "
                + "VALUES(@Id,@UserName,@UserPassword,@Email)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@UserName", model.UserName);
            cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);
            cmds.Add(cmd);
        }
        public void AddUsers(List<User> models)
        {
            Users.AddRange(models);

            foreach (var model in models)
            {
                string sql = "INSERT INTO User (Id,UserName,UserPassword,Email) "
                    + "VALUES(@Id,@UserName,@UserPassword,@Email)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);
                cmds.Add(cmd);
            }
        }
        public void EditUser(User model)
        {
            string sql =
                "UPDATE User " +
                "SET UserName=@UserName,UserPassword=@UserPassword,Email=@Email " +
                "WHERE Id=@Id";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@UserName", model.UserName);
            cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);
            cmds.Add(cmd);
        }
        #endregion


        #region Account
        public List<Account> Accounts { get; set; }
        public void AddAccount(Account model)
        {
            Accounts.Add(model);

            string sql = "INSERT INTO Account (Id,Type,IsAllowLoad,AccountNumber,Password,CustomerName,FuturesCompanyName,UserId) "
+ "VALUES(@Id,@Type,@IsAllowLoad,@AccountNumber,@Password,@CustomerName,@FuturesCompanyName,@UserId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
            cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
            cmd.Parameters.AddWithValue("@FuturesCompanyName", model.FuturesCompanyName);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@IsAllowLoad", model.IsAllowLoad);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            cmd.Parameters.AddWithValue("@Type", model.Type);
            cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
            cmds.Add(cmd);
        }
        public void AddAccounts(List<Account> models)
        {
            Accounts.AddRange(models);

            foreach (var model in models)
            {
                string sql = "INSERT INTO Account (Id,Type,IsAllowLoad,AccountNumber,Password,CustomerName,FuturesCompanyName,UserId) "
    + "VALUES(@Id,@Type,@IsAllowLoad,@AccountNumber,@Password,@CustomerName,@FuturesCompanyName,@UserId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
                cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                cmd.Parameters.AddWithValue("@FuturesCompanyName", model.FuturesCompanyName);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@IsAllowLoad", model.IsAllowLoad);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
                cmds.Add(cmd);
            }
        }
        public void EditAccount(Account model)
        {
            string sql = "UPDATE Account " +
                "SET Type=@Type,IsAllowLoad=@IsAllowLoad,AccountNumber=@AccountNumber,Password=@Password,CustomerName=@CustomerName,FuturesCompanyName=@FuturesCompanyName,UserId=@UserId " +
                "WHERE Id=@Id";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
            cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
            cmd.Parameters.AddWithValue("@FuturesCompanyName", model.FuturesCompanyName);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@IsAllowLoad", model.IsAllowLoad);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            cmd.Parameters.AddWithValue("@Type", model.Type);
            cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
            cmds.Add(cmd);
        }
        #endregion


        #region FundStatus
        public List<FundStatus> FundStatus { get; set; }
        public void AddFundStatus(FundStatus model)
        {
            FundStatus.Add(model);

            string sql = "INSERT INTO FundStatus (Id,Date,YesterdayBalance,CustomerRights,Remittance,MatterDeposit,ClosedProfit,Margin,Commission,FreeMargin,TodayBalance,VentureFactor,FloatingProfit,AdditionalMargin,SettlementType,AccountId) "
+ "VALUES(@Id,@Date,@YesterdayBalance,@CustomerRights,@Remittance,@MatterDeposit,@ClosedProfit,@Margin,@Commission,@FreeMargin,@TodayBalance,@VentureFactor,@FloatingProfit,@AdditionalMargin,@SettlementType,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@AdditionalMargin", model.AdditionalMargin);
            cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
            cmd.Parameters.AddWithValue("@Commission", model.Commission);
            cmd.Parameters.AddWithValue("@CustomerRights", model.CustomerRights);
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@FloatingProfit", model.FloatingProfit);
            cmd.Parameters.AddWithValue("@FreeMargin", model.FreeMargin);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Margin", model.Margin);
            cmd.Parameters.AddWithValue("@MatterDeposit", model.MatterDeposit);
            cmd.Parameters.AddWithValue("@Remittance", model.Remittance);
            cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
            cmd.Parameters.AddWithValue("@TodayBalance", model.TodayBalance);
            cmd.Parameters.AddWithValue("@VentureFactor", model.VentureFactor);
            cmd.Parameters.AddWithValue("@YesterdayBalance", model.YesterdayBalance);
            cmds.Add(cmd);
        }
        public void AddFundStatus(List<FundStatus> models)
        {
            FundStatus.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO FundStatus (Id,Date,YesterdayBalance,CustomerRights,Remittance,MatterDeposit,ClosedProfit,Margin,Commission,FreeMargin,TodayBalance,VentureFactor,FloatingProfit,AdditionalMargin,SettlementType,AccountId) "
    + "VALUES(@Id,@Date,@YesterdayBalance,@CustomerRights,@Remittance,@MatterDeposit,@ClosedProfit,@Margin,@Commission,@FreeMargin,@TodayBalance,@VentureFactor,@FloatingProfit,@AdditionalMargin,@SettlementType,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@AdditionalMargin", model.AdditionalMargin);
                cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
                cmd.Parameters.AddWithValue("@Commission", model.Commission);
                cmd.Parameters.AddWithValue("@CustomerRights", model.CustomerRights);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@FloatingProfit", model.FloatingProfit);
                cmd.Parameters.AddWithValue("@FreeMargin", model.FreeMargin);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Margin", model.Margin);
                cmd.Parameters.AddWithValue("@MatterDeposit", model.MatterDeposit);
                cmd.Parameters.AddWithValue("@Remittance", model.Remittance);
                cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
                cmd.Parameters.AddWithValue("@TodayBalance", model.TodayBalance);
                cmd.Parameters.AddWithValue("@VentureFactor", model.VentureFactor);
                cmd.Parameters.AddWithValue("@YesterdayBalance", model.YesterdayBalance);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region Remittance
        public List<Remittance> Remittances { get; set; }
        public void AddRemittance(Remittance model)
        {
            Remittances.Add(model);

            string sql = "INSERT INTO Remittance (Id,Date,Deposit,WithDrawal,Type,Summary,AccountId) "
            + "VALUES(@Id,@Date,@Deposit,@WithDrawal,@Type,@Summary,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@Deposit", model.Deposit);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Summary", model.Summary);
            cmd.Parameters.AddWithValue("@Type", model.Type);
            cmd.Parameters.AddWithValue("@WithDrawal", model.WithDrawal);
            cmds.Add(cmd);
        }
        public void AddRemittances(List<Remittance> models)
        {
            Remittances.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO Remittance (Id,Date,Deposit,WithDrawal,Type,Summary,AccountId) "
                + "VALUES(@Id,@Date,@Deposit,@WithDrawal,@Type,@Summary,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@Deposit", model.Deposit);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Summary", model.Summary);
                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@WithDrawal", model.WithDrawal);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region Trade
        public List<Trade> Trades { get; set; }
        public void AddTrade(Trade model)
        {
            Trades.Add(model);

            string sql = "INSERT INTO Trade (Id,Date ,Item ,BS ,OC ,Price ,Size ,Amount ,Commission ,SH ,ClosedProfit,AccountId) "
                + "VALUES(@Id,@Date ,@Item ,@BS ,@OC ,@Price ,@Size ,@Amount ,@Commission ,@SH ,@ClosedProfit,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@BS", model.BS);
            cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
            cmd.Parameters.AddWithValue("@Commission", model.Commission);
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Item", model.Item);
            cmd.Parameters.AddWithValue("@OC", model.OC);
            cmd.Parameters.AddWithValue("@Price", model.Price);
            cmd.Parameters.AddWithValue("@SH", model.SH);
            cmd.Parameters.AddWithValue("@Size", model.Size);
            cmds.Add(cmd);
        }
        public void AddTrades(List<Trade> models)
        {
            Trades.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO Trade (Id,Date ,Item ,BS ,OC ,Price ,Size ,Amount ,Commission ,SH ,ClosedProfit,AccountId) "
                    + "VALUES(@Id,@Date ,@Item ,@BS ,@OC ,@Price ,@Size ,@Amount ,@Commission ,@SH ,@ClosedProfit,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@Amount", model.Amount);
                cmd.Parameters.AddWithValue("@BS", model.BS);
                cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
                cmd.Parameters.AddWithValue("@Commission", model.Commission);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Item", model.Item);
                cmd.Parameters.AddWithValue("@OC", model.OC);
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@SH", model.SH);
                cmd.Parameters.AddWithValue("@Size", model.Size);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region Position
        public List<Position> Positions { get; set; }
        public void AddPosition(Position model)
        {
            Positions.Add(model);

            string sql = "INSERT INTO Position (Id,Date,Item,BuySize,BuyAveragePrice,SaleSize,SaleAveragePrice,YesterdaySettlementPrice,TodaySettlementPrice,Profit,Margin,SH,SettlementType,AccountId) "
                + "VALUES(@Id,@Date,@Item,@BuySize,@BuyAveragePrice,@SaleSize,@SaleAveragePrice,@YesterdaySettlementPrice,@TodaySettlementPrice,@Profit,@Margin,@SH,@SettlementType,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@Item", model.Item);
            cmd.Parameters.AddWithValue("@BuySize", model.BuySize);
            cmd.Parameters.AddWithValue("@BuyAveragePrice", model.BuyAveragePrice);
            cmd.Parameters.AddWithValue("@SaleSize", model.SaleSize);
            cmd.Parameters.AddWithValue("@SaleAveragePrice", model.SaleAveragePrice);
            cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
            cmd.Parameters.AddWithValue("@TodaySettlementPrice", model.TodaySettlementPrice);
            cmd.Parameters.AddWithValue("@Profit", model.Profit);
            cmd.Parameters.AddWithValue("@Margin", model.Margin);
            cmd.Parameters.AddWithValue("@SH", model.SH);
            cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmds.Add(cmd);
        }
        public void AddPositions(List<Position> models)
        {
            Positions.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO Position (Id,Date,Item,BuySize,BuyAveragePrice,SaleSize,SaleAveragePrice,YesterdaySettlementPrice,TodaySettlementPrice,Profit,Margin,SH,SettlementType,AccountId) "
                    + "VALUES(@Id,@Date,@Item,@BuySize,@BuyAveragePrice,@SaleSize,@SaleAveragePrice,@YesterdaySettlementPrice,@TodaySettlementPrice,@Profit,@Margin,@SH,@SettlementType,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@BuyAveragePrice", model.BuyAveragePrice);
                cmd.Parameters.AddWithValue("@BuySize", model.BuySize);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Item", model.Item);
                cmd.Parameters.AddWithValue("@Margin", model.Margin);
                cmd.Parameters.AddWithValue("@Profit", model.Profit);
                cmd.Parameters.AddWithValue("@SaleAveragePrice", model.SaleAveragePrice);
                cmd.Parameters.AddWithValue("@SaleSize", model.SaleSize);
                cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
                cmd.Parameters.AddWithValue("@SH", model.SH);
                cmd.Parameters.AddWithValue("@TodaySettlementPrice", model.TodaySettlementPrice);
                cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region CommoditySummarization
        public List<CommoditySummarization> CommoditySummarizations { get; set; }
        public void AddCommoditySummarization(CommoditySummarization model)
        {
            CommoditySummarizations.Add(model);

            string sql = "INSERT INTO CommoditySummarization (Id,Date,Commodity,Size,Amount,Commission,ClosedProfit,AccountId) "
                + "VALUES(@Id,@Date,@Commodity,@Size,@Amount,@Commission,@ClosedProfit,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
            cmd.Parameters.AddWithValue("@Commission", model.Commission);
            cmd.Parameters.AddWithValue("@Commodity", model.Commodity);
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Size", model.Size);
            cmds.Add(cmd);
        }
        public void AddCommoditySummarizations(List<CommoditySummarization> models)
        {
            CommoditySummarizations.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO CommoditySummarization (Id,Date,Commodity,Size,Amount,Commission,ClosedProfit,AccountId) "
                    + "VALUES(@Id,@Date,@Commodity,@Size,@Amount,@Commission,@ClosedProfit,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@Amount", model.Amount);
                cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
                cmd.Parameters.AddWithValue("@Commission", model.Commission);
                cmd.Parameters.AddWithValue("@Commodity", model.Commodity);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Size", model.Size);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region TradeDetail
        public List<TradeDetail> TradeDetails { get; set; }
        public void AddTradeDetail(TradeDetail model)
        {
            TradeDetails.Add(model);

            string sql = "INSERT INTO TradeDetail (Id,ActualTime ,Item ,Ticket ,BS ,SH ,Price ,Size ,Amount ,OC ,Commission ,ClosedProfit,AccountId) "
                + "VALUES(@Id,@ActualTime ,@Item ,@Ticket ,@BS ,@SH ,@Price ,@Size ,@Amount ,@OC ,@Commission ,@ClosedProfit,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@ActualTime", model.ActualTime);
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@BS", model.BS);
            cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
            cmd.Parameters.AddWithValue("@Commission", model.Commission);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Item", model.Item);
            cmd.Parameters.AddWithValue("@OC", model.OC);
            cmd.Parameters.AddWithValue("@Price", model.Price);
            cmd.Parameters.AddWithValue("@SH", model.SH);
            cmd.Parameters.AddWithValue("@Size", model.Size);
            cmd.Parameters.AddWithValue("@Ticket", model.Ticket);
            cmds.Add(cmd);
        }
        public void AddTradeDetails(List<TradeDetail> models)
        {
            TradeDetails.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO TradeDetail (Id,ActualTime ,Item ,Ticket ,BS ,SH ,Price ,Size ,Amount ,OC ,Commission ,ClosedProfit,AccountId) "
                    + "VALUES(@Id,@ActualTime ,@Item ,@Ticket ,@BS ,@SH ,@Price ,@Size ,@Amount ,@OC ,@Commission ,@ClosedProfit,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@ActualTime", model.ActualTime);
                cmd.Parameters.AddWithValue("@Amount", model.Amount);
                cmd.Parameters.AddWithValue("@BS", model.BS);
                cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
                cmd.Parameters.AddWithValue("@Commission", model.Commission);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Item", model.Item);
                cmd.Parameters.AddWithValue("@OC", model.OC);
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@SH", model.SH);
                cmd.Parameters.AddWithValue("@Size", model.Size);
                cmd.Parameters.AddWithValue("@Ticket", model.Ticket);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region ClosedTradeDetail
        public List<ClosedTradeDetail> ClosedTradeDetails { get; set; }
        public void AddClosedTradeDetail(ClosedTradeDetail model)
        {
            ClosedTradeDetails.Add(model);

            string sql = "INSERT INTO ClosedTradeDetail (Id,ActualDate,Item,TicketForClose,BS,PriceForClose,PriceForOpen,Size,YesterdaySettlementPrice,ClosedProfit,TicketForOpen,AccountId) "
                + "VALUES(@Id,@ActualDate,@Item,@TicketForClose,@BS,@PriceForClose,@PriceForOpen,@Size,@YesterdaySettlementPrice,@ClosedProfit,@TicketForOpen,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@ActualDate", model.ActualDate);
            cmd.Parameters.AddWithValue("@BS", model.BS);
            cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Item", model.Item);
            cmd.Parameters.AddWithValue("@PriceForClose", model.PriceForClose);
            cmd.Parameters.AddWithValue("@PriceForOpen", model.PriceForOpen);
            cmd.Parameters.AddWithValue("@Size", model.Size);
            cmd.Parameters.AddWithValue("@TicketForClose", model.TicketForClose);
            cmd.Parameters.AddWithValue("@TicketForOpen", model.TicketForOpen);
            cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
            cmds.Add(cmd);
        }
        public void AddClosedTradeDetails(List<ClosedTradeDetail> models)
        {
            ClosedTradeDetails.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO ClosedTradeDetail (Id,ActualDate,Item,TicketForClose,BS,PriceForClose,PriceForOpen,Size,YesterdaySettlementPrice,ClosedProfit,TicketForOpen,AccountId) "
                    + "VALUES(@Id,@ActualDate,@Item,@TicketForClose,@BS,@PriceForClose,@PriceForOpen,@Size,@YesterdaySettlementPrice,@ClosedProfit,@TicketForOpen,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@ActualDate", model.ActualDate);
                cmd.Parameters.AddWithValue("@BS", model.BS);
                cmd.Parameters.AddWithValue("@ClosedProfit", model.ClosedProfit);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Item", model.Item);
                cmd.Parameters.AddWithValue("@PriceForClose", model.PriceForClose);
                cmd.Parameters.AddWithValue("@PriceForOpen", model.PriceForOpen);
                cmd.Parameters.AddWithValue("@Size", model.Size);
                cmd.Parameters.AddWithValue("@TicketForClose", model.TicketForClose);
                cmd.Parameters.AddWithValue("@TicketForOpen", model.TicketForOpen);
                cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region PositionDetail
        public List<PositionDetail> PositionDetails { get; set; }
        public void AddPositionDetail(PositionDetail model)
        {
            PositionDetails.Add(model);

            string sql = "INSERT INTO PositionDetail (Id,DateForPosition,DateForActual,Item,Ticket,BuySize,BuyPrice,SaleSize,SalePrice,YesterdaySettlementPrice,TodaySettlementPrice,Profit,SH,TradeCode,SettlementType,AccountId) "
                + "VALUES(@Id,@DateForPosition,@DateForActual,@Item,@Ticket,@BuySize,@BuyPrice,@SaleSize,@SalePrice,@YesterdaySettlementPrice,@TodaySettlementPrice,@Profit,@SH,@TradeCode,@SettlementType,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@BuyPrice", model.BuyPrice);
            cmd.Parameters.AddWithValue("@BuySize", model.BuySize);
            cmd.Parameters.AddWithValue("@DateForActual", model.DateForActual);
            cmd.Parameters.AddWithValue("@DateForPosition", model.DateForPosition);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Item", model.Item);
            cmd.Parameters.AddWithValue("@Profit", model.Profit);
            cmd.Parameters.AddWithValue("@SalePrice", model.SalePrice);
            cmd.Parameters.AddWithValue("@SaleSize", model.SaleSize);
            cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
            cmd.Parameters.AddWithValue("@SH", model.SH);
            cmd.Parameters.AddWithValue("@Ticket", model.Ticket);
            cmd.Parameters.AddWithValue("@TodaySettlementPrice", model.TodaySettlementPrice);
            cmd.Parameters.AddWithValue("@TradeCode", model.TradeCode);
            cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
            cmds.Add(cmd);
        }
        public void AddPositionDetails(List<PositionDetail> models)
        {
            PositionDetails.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO PositionDetail (Id,DateForPosition,DateForActual,Item,Ticket,BuySize,BuyPrice,SaleSize,SalePrice,YesterdaySettlementPrice,TodaySettlementPrice,Profit,SH,TradeCode,SettlementType,AccountId) "
    + "VALUES(@Id,@DateForPosition,@DateForActual,@Item,@Ticket,@BuySize,@BuyPrice,@SaleSize,@SalePrice,@YesterdaySettlementPrice,@TodaySettlementPrice,@Profit,@SH,@TradeCode,@SettlementType,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@BuyPrice", model.BuyPrice);
                cmd.Parameters.AddWithValue("@BuySize", model.BuySize);
                cmd.Parameters.AddWithValue("@DateForActual", model.DateForActual);
                cmd.Parameters.AddWithValue("@DateForPosition", model.DateForPosition);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Item", model.Item);
                cmd.Parameters.AddWithValue("@Profit", model.Profit);
                cmd.Parameters.AddWithValue("@SalePrice", model.SalePrice);
                cmd.Parameters.AddWithValue("@SaleSize", model.SaleSize);
                cmd.Parameters.AddWithValue("@SettlementType", model.SettlementType);
                cmd.Parameters.AddWithValue("@SH", model.SH);
                cmd.Parameters.AddWithValue("@Ticket", model.Ticket);
                cmd.Parameters.AddWithValue("@TodaySettlementPrice", model.TodaySettlementPrice);
                cmd.Parameters.AddWithValue("@TradeCode", model.TradeCode);
                cmd.Parameters.AddWithValue("@YesterdaySettlementPrice", model.YesterdaySettlementPrice);
                cmds.Add(cmd);
            }
        }
        #endregion


        #region Parameter
        public List<Parameter> Parameters { get; set; }
        public void AddParameter(Parameter model)
        {
            Parameters.Add(model);

            string sql = "INSERT INTO Parameter (Id,Name,Value,UserId) "
            + "VALUES(@Id,@Name,@Value,@UserId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
            cmd.Parameters.AddWithValue("@Value", model.Value);
            cmds.Add(cmd);
        }
        public void AddParameters(List<Parameter> models)
        {
            Parameters.AddRange(models);
            foreach (var model in models)
            {
                string sql = "INSERT INTO Parameter (Id,Name,Value,UserId) "
                + "VALUES(@Id,@Name,@Value,@UserId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
                cmd.Parameters.AddWithValue("@Value", model.Value);
                cmds.Add(cmd);
            }
        }
        public void EditParameter(Parameter model)
        {
            string sql =
                "UPDATE Parameter " +
                "SET Name=@Name,Value=@Value,UserId=@UserId " +
                "WHERE Id=@Id";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@UserId", model.UserId.ToString());
            cmd.Parameters.AddWithValue("@Value", model.Value);
            cmds.Add(cmd);
        }
        #endregion


        #region Stock
        public List<Stock> Stocks { get; set; }
        public void AddStock(Stock model)
        {
            Stocks.Add(model);

            string sql = "INSERT INTO Stock (Id,Date,Open,High,Low,Close,Volume,AccountId) "
                + "VALUES(@Id,@Date,@Open,@High,@Low,@Close,@Volume,@AccountId)";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
            cmd.Parameters.AddWithValue("@Close", model.Close);
            cmd.Parameters.AddWithValue("@Date", model.Date);
            cmd.Parameters.AddWithValue("@High", model.High);
            cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
            cmd.Parameters.AddWithValue("@Low", model.Low);
            cmd.Parameters.AddWithValue("@Open", model.Open);
            cmd.Parameters.AddWithValue("@Volume", model.Volume);
            cmds.Add(cmd);
        }
        public void AddStocks(List<Stock> models)
        {
            Stocks.AddRange(models);

            foreach (var model in models)
            {
                string sql = "INSERT INTO Stock (Id,Date,Open,High,Low,Close,Volume,AccountId) "
                    + "VALUES(@Id,@Date,@Open,@High,@Low,@Close,@Volume,@AccountId)";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@Close", model.Close);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@High", model.High);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Low", model.Low);
                cmd.Parameters.AddWithValue("@Open", model.Open);
                cmd.Parameters.AddWithValue("@Volume", model.Volume);
                cmds.Add(cmd);
            }
        }
        public void UpdateStocks(IEnumerable<Stock> models)
        {
            string sql = "UPDATE Stock " +
                "SET Date=@Date,Open=@Open,High=@High,Low=@Low,Close=@Close,Volume=@Volume,AccountId=@AccountId " +
                "WHERE Id=@Id";
            foreach (var model in models)
            {
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.Parameters.AddWithValue("@AccountId", model.AccountId.ToString());
                cmd.Parameters.AddWithValue("@Close", model.Close);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@High", model.High);
                cmd.Parameters.AddWithValue("@Id", model.Id.ToString());
                cmd.Parameters.AddWithValue("@Low", model.Low);
                cmd.Parameters.AddWithValue("@Open", model.Open);
                cmd.Parameters.AddWithValue("@Volume", model.Volume);
                cmds.Add(cmd);
            }
        }
        #endregion

        public int SaveChanged()
        {
            SQLiteConnection conn = null;
            SQLiteTransaction tran = null;
            try
            {
                conn = new SQLiteConnection(connectionStr);//创建数据库实例，指定文件位置  
                conn.Open();//打开数据库，若文件不存在会自动创建  
                tran = conn.BeginTransaction();
                foreach (var cmd in cmds)
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                cmds.Clear();
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
            return 0;
        }


        public void Delete(DateTime dateTime, Guid accountId, params Type[] types)
        {
            foreach (var type in types)
            {
                SQLiteCommand cmd = null;

                if (type == typeof(Commodity))
                {
                    continue;
                }
                else if (type == typeof(User))
                {
                    continue;
                }
                else if (type == typeof(Account))
                {
                    continue;
                }
                else if (type == typeof(ClosedTradeDetail))
                {
                    string sql = "DELETE FROM ClosedTradeDetail WHERE AccountId=@AccountId AND ActualDate>=@Date";
                    cmd = new SQLiteCommand(sql);
                    ClosedTradeDetails.RemoveAll(m => m.AccountId == accountId && m.ActualDate >= dateTime.Date);
                }
                else if (type == typeof(CommoditySummarization))
                {
                    string sql = "DELETE FROM CommoditySummarization WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    CommoditySummarizations.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else if (type == typeof(FundStatus))
                {
                    string sql = "DELETE FROM FundStatus WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    FundStatus.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else if (type == typeof(Parameter))
                {
                    continue;
                }
                else if (type == typeof(PositionDetail))
                {
                    string sql = "DELETE FROM PositionDetail WHERE AccountId=@AccountId AND DateForPosition>=@Date";
                    cmd = new SQLiteCommand(sql);
                    PositionDetails.RemoveAll(m => m.AccountId == accountId && m.DateForPosition >= dateTime.Date);
                }
                else if (type == typeof(Position))
                {
                    string sql = "DELETE FROM Position WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    Positions.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else if (type == typeof(Remittance))
                {
                    string sql = "DELETE FROM Remittance WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    Remittances.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else if (type == typeof(Stock))
                {
                    string sql = "DELETE FROM Stock WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    Stocks.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else if (type == typeof(TradeDetail))
                {
                    string sql = "DELETE FROM TradeDetail WHERE AccountId=@AccountId AND ActualTime>=@Date";
                    cmd = new SQLiteCommand(sql);
                    TradeDetails.RemoveAll(m => m.AccountId == accountId && m.ActualTime >= dateTime.Date);
                }
                else if (type == typeof(Trade))
                {
                    string sql = "DELETE FROM Trade WHERE AccountId=@AccountId AND Date>=@Date";
                    cmd = new SQLiteCommand(sql);
                    Trades.RemoveAll(m => m.AccountId == accountId && m.Date >= dateTime.Date);
                }
                else
                {
                    continue;
                }
                cmd.Parameters.AddWithValue("@AccountId", accountId.ToString());
                cmd.Parameters.AddWithValue("@Date", dateTime.Date);
                cmds.Add(cmd);
            }
        }

        #region 构造函数

        public StatementContext()
        {
            try
            {
                StatementStream stream = new StatementStream();
                Commoditys = stream.GetCommoditys();
                Accounts = stream.GetAccounts();
                ClosedTradeDetails = stream.GetClosedTradeDetails();
                CommoditySummarizations = stream.GetCommoditySummarizations();
                FundStatus = stream.GetFundStatus();
                Parameters = stream.GetParameters();
                PositionDetails = stream.GetPositionDetails();
                Positions = stream.GetPositions();
                Remittances = stream.GetRemittances();
                Stocks = stream.GetStocks();
                TradeDetails = stream.GetTradeDetails();
                Trades = stream.GetTrades();
                Users = stream.GetUsers();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StatementContext(params Type[] types)
        {
            try
            {
                StatementStream stream = new StatementStream();

                foreach (var type in types)
                {
                    if (type == typeof(Commodity))
                    {
                        Commoditys = stream.GetCommoditys();
                    }
                    else if (type == typeof(User))
                    {
                        Users = stream.GetUsers();
                    }
                    else if (type == typeof(Account))
                    {
                        Accounts = stream.GetAccounts();
                    }
                    else if (type == typeof(ClosedTradeDetail))
                    {
                        ClosedTradeDetails = stream.GetClosedTradeDetails();
                    }
                    else if (type == typeof(CommoditySummarization))
                    {
                        CommoditySummarizations = stream.GetCommoditySummarizations();
                    }
                    else if (type == typeof(FundStatus))
                    {
                        FundStatus = stream.GetFundStatus();
                    }
                    else if (type == typeof(Parameter))
                    {
                        Parameters = stream.GetParameters();
                    }
                    else if (type == typeof(PositionDetail))
                    {
                        PositionDetails = stream.GetPositionDetails();
                    }
                    else if (type == typeof(Position))
                    {
                        Positions = stream.GetPositions();
                    }
                    else if (type == typeof(Remittance))
                    {
                        Remittances = stream.GetRemittances();
                    }
                    else if (type == typeof(Stock))
                    {
                        Stocks = stream.GetStocks();
                    }
                    else if (type == typeof(TradeDetail))
                    {
                        TradeDetails = stream.GetTradeDetails();
                    }
                    else if (type == typeof(Trade))
                    {
                        Trades = stream.GetTrades();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        public void Dispose()
        {
            if (Commoditys != null)
                Commoditys.Clear();
            if (Users != null)
                Users.Clear();
            if (Accounts != null)
                Accounts.Clear();
            if (ClosedTradeDetails != null)
                ClosedTradeDetails.Clear();
            if (CommoditySummarizations != null)
                CommoditySummarizations.Clear();
            if (FundStatus != null)
                FundStatus.Clear();
            if (Parameters != null)
                Parameters.Clear();
            if (PositionDetails != null)
                PositionDetails.Clear();
            if (Positions != null)
                Positions.Clear();
            if (Remittances != null)
                Remittances.Clear();
            if (Stocks != null)
                Stocks.Clear();
            if (TradeDetails != null)
                TradeDetails.Clear();
            if (Trades != null)
                Trades.Clear();

            Commoditys = null;
            Users = null;
            Accounts = null;
            ClosedTradeDetails = null;
            CommoditySummarizations = null;
            FundStatus = null;
            Parameters = null;
            PositionDetails = null;
            Positions = null;
            Remittances = null;
            Stocks = null;
            TradeDetails = null;
            Trades = null;
        }

    }
}
