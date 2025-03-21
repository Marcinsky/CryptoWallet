using Microsoft.AspNetCore.Mvc;
using CryptoWallet.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Pobierz wszystkie portfele
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets()
        {
            return await _context.Wallets.ToListAsync();
        }

        // Pobierz konkretny portfel po ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetWallet(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);

            if (wallet == null)
            {
                return NotFound(new { message = "Portfel nie istnieje" });
            }

            return wallet;
        }

        // Pobierz portfel użytkownika (jeśli masz logowanie)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetUserWallet(int userId)
        {
            var wallets = await _context.Wallets.Where(w => w.UserId == userId).ToListAsync();

            if (!wallets.Any())
            {
                return NotFound(new { message = "Brak portfela dla tego użytkownika" });
            }

            return wallets;
        }

        // Dodaj nowy wpis do portfela
        [HttpPost]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            Console.WriteLine($"Dodawanie coina: {wallet.CoinName}, Cena: {wallet.EntryPriceUSD}, Wpłata: {wallet.AmountOwned}");

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWallet), new { id = wallet.WalletId }, wallet);
        }

        // Edytuj wpis w portfelu
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWallet(int id, Wallet updatedWallet)
        {
            if (id != updatedWallet.WalletId)
            {
                return BadRequest(new { message = "ID portfela nie zgadza się" });
            }

            var existingWallet = await _context.Wallets.FindAsync(id);
            if (existingWallet == null)
            {
                return NotFound(new { message = "Portfel nie istnieje" });
            }

            existingWallet.CoinName = updatedWallet.CoinName;
            existingWallet.EntryPriceUSD = updatedWallet.EntryPriceUSD;
            existingWallet.AmountOwned = updatedWallet.AmountOwned;
            existingWallet.CurrentPriceUSD = updatedWallet.CurrentPriceUSD;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Portfel zaktualizowany" });
        }

        // Usuń wpis z portfela
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound(new { message = "Portfel nie istnieje" });
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Portfel usunięty" });
        }
    }
}
