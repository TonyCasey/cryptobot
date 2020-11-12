using System.Collections.Generic;
using System.Configuration;
using CryptoBot.Database;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Market;
using NLog.Internal;

namespace CryptoBot.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CryptoBotDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CryptoBotDbContext context)
        {
            SeedData.Seed(context);

            base.Seed(context);
        }
    }
}
