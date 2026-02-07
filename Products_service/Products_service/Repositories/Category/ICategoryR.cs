namespace Products_service.Repositories.Category
{
    public interface ICategoryR
    {
        public Task<List<Models.Category>> GetCategories();
        public Task<Models.Category> GetCategory(int id);
        public Task AddCategry(Models.Category category);
    }
}
