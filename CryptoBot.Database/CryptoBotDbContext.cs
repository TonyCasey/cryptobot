using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CryptoBot.Model.Domain;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Lookup;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using NLog;

namespace CryptoBot.Database
{
    public class CryptoBotDbContext : DbContext
    {
        private Logger Logger { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Coin> Coins { get; set; }
        public virtual DbSet<Exchange> Exchanges { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Candle> Candles { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<ApiSetting> ApiSettings { get; set; }
        public virtual DbSet<Safety> Safeties { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<RuleSet> RuleSets { get; set; }
        public virtual DbSet<Indicator> Indicators { get; set; }
        public virtual DbSet<Bot> Bots { get; set; }
        public virtual DbSet<Enumeration> Enumerations { get; set; }
        public virtual DbSet<MessagingApp> MessagingApps { get; set; }
        public virtual DbSet<MessagingAppSettings> MessagingAppSettings { get; set; }

        public CryptoBotDbContext(DbContextOptions<CryptoBotDbContext> options) : base(options)
        {
            
        }

        /// <summary>
        /// SaveChanges with AddUserAndTimestamp extension
        /// Fills CreationTime, CreationUser, LastUpdateTime, LastUpdateUser fields
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            AddUserAndTimestamp();
            try
            {
                return base.SaveChanges();
            }
            catch (Exception e)
            {
                Logger = LogManager.GetCurrentClassLogger();
                Logger.Error(e.Message);
                Logger.Error(e.InnerException);
                throw;
            }
        }

        /// <summary>
        /// Fills CreationTime, CreationUser, LastUpdateTime, LastUpdateUser fields of the entity that is BaseEntity
        /// </summary>
        private void AddUserAndTimestamp()
        {
            var entities = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified || e.State == EntityState.Added));

            var currentUsername = "CryptoBot";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreationTime = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).CreationUser = currentUsername;
                }
                else
                {
                    ((BaseEntity)entity.Entity).LastUpdateTime = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).LastUpdateUser = currentUsername;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bot>()
                .HasOne(c => c.BaseCoin)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Bot>()
                .HasOne(c => c.Coin)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal properties with precision
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(38);
                        property.SetScale(18);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}