namespace CryptoWallet.Models
{
    public class CryptoModel
    {
        public int Id { get; set; }
        public string CoinName { get; set; } = string.Empty; 
        public decimal EntryPrice { get; set; }             
        public decimal DepositUSD { get; set; }             
        public decimal CurrentPrice { get; set; }            
        public decimal AmountOwned => DepositUSD / EntryPrice; 
        public decimal TotalValue => AmountOwned * CurrentPrice; 

    }
}
