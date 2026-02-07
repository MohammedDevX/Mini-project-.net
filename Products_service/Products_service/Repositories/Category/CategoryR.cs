using Microsoft.EntityFrameworkCore;
using Products_service.Data;

namespace Products_service.Repositories.Category
{
    public class CategoryR: ICategoryR
    {
        private AppDbContext context;
        public CategoryR(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Models.Category>> GetCategories()
        {
            List<Models.Category> categories = await context.Category.ToListAsync();
            return categories;
        } 

        public async Task<Models.Category> GetCategory(int id)
        {
            Models.Category category = await context.Category.FindAsync(id);
            return category;
        }
        public async Task AddCategry(Models.Category category)
        {
            await context.Category.AddAsync(category);
            await context.SaveChangesAsync();
        }
    }
}
