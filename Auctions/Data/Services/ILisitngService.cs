using Auctions.Models;

namespace Auctions.Data.Services
{
    public interface IListingsService
    {
        IQueryable<Listing> GetAll();
        Task Add(Listing listing);
        Task<Listing> GetById(int? id);
        Task SaveChanges();

        Task CloseExpiredListings(); //NEW
        Task CloseBiddingById(int id); //NEW

        Task DeleteAsync(Listing listing);


    }
}