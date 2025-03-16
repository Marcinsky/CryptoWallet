using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CryptoWallet.Models;
using CryptoWallet.Services;
using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Pages
{
    public class CryptoPageModel : PageModel
    {
        private  CryptoService _cryptoService;
        private  ApplicationDbContext _context;

        public CryptoPageModel(CryptoService cryptoService, ApplicationDbContext context)
        {
            _cryptoService = cryptoService;
            _context = context;
            Coins = new List<CryptoModel>();
        }

        [BindProperty]
        public string CoinName { get; set; } = string.Empty; // Nazwa kryptowaluty

        [BindProperty]
        public decimal EntryPrice { get; set; } // Cena wejœcia

        [BindProperty]
        public decimal DepositUSD { get; set; } // Kwota wp³acona w z³otówkach

        [BindProperty]
        public CryptoModel EditCoin { get; set; } // Dane edytowanego rekordu

        public List<CryptoModel> Coins { get; private set; } // Lista kryptowalut u¿ytkownika

        public async Task OnGetAsync()
        {
            Coins = await _context.Wallets.Select(w => new CryptoModel
            {
                CoinName = w.CoinName,
                EntryPrice = w.EntryPriceUSD,
                DepositUSD = w.AmountOwned,
                CurrentPrice = w.CurrentPriceUSD 
            })
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);

            if (wallet == null)
            {
                return NotFound(); // Jeœli rekord nie istnieje
            }

            // Wype³nij dane do edycji
            EditCoin = new CryptoModel
            {
                Id = wallet.CoinId,
                CoinName = wallet.CoinName,
                EntryPrice = wallet.EntryPriceUSD,
                DepositUSD = wallet.AmountOwned,
                CurrentPrice = wallet.CurrentPriceUSD
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            var wallet = await _context.Wallets.FindAsync(EditCoin.Id);

            if (wallet == null)
            {
                return NotFound();
            }

            // Aktualizuj rekord
            wallet.CoinName = EditCoin.CoinName;
            wallet.EntryPriceUSD = EditCoin.EntryPrice;
            wallet.AmountOwned = EditCoin.DepositUSD;

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);

            if (wallet == null)
            {
                return NotFound(); 
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(CoinName) || EntryPrice <= 0 || DepositUSD <= 0)
            {
                return Page(); 
            }

            try
            {
                // Pobierz aktualn¹ cenê kryptowaluty z CoinGecko
                decimal currentPrice = await _cryptoService.GetCoinPriceAsync(CoinName);

                // Dodajemy kryptowalutê do listy
                var newWallet = new Wallet 
                {
                    CoinName = CoinName,
                    EntryPriceUSD = EntryPrice,
                    AmountOwned = DepositUSD,
                    CurrentPriceUSD = currentPrice
                };

               
                _context.Wallets.Add(newWallet);
                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Obs³uga b³êdu (np. problem z API)
                ModelState.AddModelError(string.Empty, "Try again later");
            }

            return Page();
        }

      

        

    }
}
