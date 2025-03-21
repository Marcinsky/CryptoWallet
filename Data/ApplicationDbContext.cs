﻿using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        
    }

    public class User
    {
        public int UserId { get; set; }
        public required string  Username { get; set; }
        public required string PasswordHash { get; set; }
    }

    public class Wallet
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public required string CoinName { get; set; }
        public decimal EntryPriceUSD { get; set; }
        public decimal AmountOwned { get; set; }
        public decimal CurrentPriceUSD { get; set; }
        public decimal TotalValueUSD  => AmountOwned * CurrentPriceUSD; 
    }

}
