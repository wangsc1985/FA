using FuturesAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    public class StatementContext : DbContext
    {
        public StatementContext() : base("ORMContext") { } //配置使用的连接名

        public DbSet<Commodity> Commodity { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<FundStatus> FundStatus { get; set; }
        public DbSet<Remittance> Remittance { get; set; }
        public DbSet<Trade> Trade { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<CommoditySummarization> CommoditySummarization { get; set; }
        public DbSet<TradeDetail> TradeDetail { get; set; }
        public DbSet<ClosedTradeDetail> ClosedTradeDetail { get; set; }
        public DbSet<PositionDetail> PositionDetail { get; set; }
        public DbSet<Parameter> Parameter { get; set; }
        public DbSet<Stock> Stock { get; set; }
    }
}
