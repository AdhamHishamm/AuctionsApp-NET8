using Auctions.Models;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Data.Services
{
    public class ListingsService : IListingsService
    {
        private readonly ApplicationDbContext _context;

        public ListingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Listing listing)
        {
            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Listing> GetAll()
        {
            var applicationDbContext = _context.Listings.Include(l => l.User);
            return applicationDbContext;
        }

        public async Task<Listing> GetById(int? id)
        {
            var listing = await _context.Listings
                .Include(l => l.User)
                .Include(l => l.Comments)
                .Include(l => l.Bids)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            return listing;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CloseExpiredListings()
        {
            var expiredListings = await _context.Listings
                .Where(l => l.EndDate <= DateTime.Now && !l.IsSold)
                .ToListAsync();

            foreach (var listing in expiredListings)
            {
                await CloseBiddingById(listing.Id);
            }
        }


        public async Task CloseBiddingById(int id)
        {
            var listing = await GetById(id);
            if (listing != null)
            {
                listing.IsSold = true;
                await SaveChanges();
            }
        }

        public async Task DeleteAsync(Listing listing)
        {
            // Check if there are any bids associated with the listing
            var bidsToDelete = await _context.Bids.Where(b => b.ListingId == listing.Id).ToListAsync();

            if (bidsToDelete != null && bidsToDelete.Any())
            {
                // Remove all bids associated with the listing
                _context.Bids.RemoveRange(bidsToDelete);
            }

            // Remove the listing itself
            _context.Listings.Remove(listing);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}