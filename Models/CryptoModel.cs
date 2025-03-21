namespace CryptoWallet.Models
{
    public class CryptoModel
    {
        public int Id { get; set; }
        public required string CoinName { get; set; } = string.Empty; 
        public decimal EntryPrice { get; set; }             
        public decimal DepositUSD { get; set; }             
        public decimal CurrentPrice { get; set; }            
        public decimal AmountOwned => EntryPrice != 0 ? Math.Round(DepositUSD / EntryPrice ,2) : 0; 
        public decimal TotalValue => CurrentPrice != 0 ? Math.Round(AmountOwned / CurrentPrice ,2) : 0;

    }
}
