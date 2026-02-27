using LunchAndLearn_AIIntegration.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LunchAndLearn_AIIntegration.Data.Repositories
{
    public class ItemRepository
    {
        readonly IDbContextFactory<DB> _db;

        public ItemRepository(IDbContextFactory<DB> db)
        {
            _db = db;
        }

        public async Task<List<Item>> GetItemsAsync()
        {
            using var _ = await _db.CreateDbContextAsync();

            return _.Items.ToList();
        }
    }
}
