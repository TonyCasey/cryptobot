using CryptoBot.Model.Domain;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Lookup;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;

namespace Api.CryptoBot.Data
{
    public class CryptoBotApiDbContext : DbContext
    {
        private readonly DbContextOptions _options;
        private readonly IHttpContextAccessor _context;

        public CryptoBotApiDbContext(DbContextOptions<CryptoBotApiDbContext> options, IHttpContextAccessor context) : base(options)
        {
            _options = options;
            _context = context;
        }

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

        /// <summary>
        /// SaveChanges with AddUserAndTimestamp extension
        /// Fills CreationTime, CreationUser, LastUpdateTime, LastUpdateUser fields
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            AddUserAndTimestamp();
            return base.SaveChanges();
        }

        /// <summary>
        /// Fills CreationTime, CreationUser, LastUpdateTime, LastUpdateUser fields of the entity that is BaseEntity
        /// </summary>
        private void AddUserAndTimestamp()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                            (e.State == EntityState.Modified || e.State == EntityState.Added));

            var currentUsername = "";
            

            if (_context != null)
            {
                
            }

            foreach (var entity in entities)
            {
                var baseEntity = ((BaseEntity)entity.Entity);
                
                if (entity.State == EntityState.Added)
                {
                    if (string.IsNullOrEmpty(currentUsername))  // this is for e.g. seeding, if the user is not available from the claim we use what is set for or the default..
                    {
                        currentUsername = baseEntity.CreationUser ?? "DefaultUser";
                    }

                    baseEntity.CreationTime = DateTime.UtcNow;
                    baseEntity.CreationUser = currentUsername;
                }
                else
                {
                    baseEntity.LastUpdateTime = DateTime.UtcNow;
                    baseEntity.LastUpdateUser = currentUsername;
                }
            }
        }

        /// <summary>
        /// OnModelCreating: removes cascade delete from FK-s
        /// </summary>
        /// <param name="modelBuilder">The model builder of the context</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            
            modelBuilder.Entity<Bot>()
                .HasMany(x => x.Positions);
            modelBuilder.Entity<Bot>()
                .HasOne(x => x.CurrentPosition);
            modelBuilder.Entity<Bot>()
                .HasOne(x => x.Coin);
            modelBuilder.Entity<Bot>()
                .HasOne(x => x.BaseCoin);
            modelBuilder.Entity<Bot>()
                .HasKey(t => t.BotId);
            
            modelBuilder.Entity<Position>()
                .HasOne(x => x.Bot);
            
            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Needed for Migrations
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["CryptoBotConnection"].ConnectionString;
                
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
