using Products_service.DTOS;
using Products_service.Models;

namespace Products_service.Mappers
{
    public class CategoryM
    {
        public static List<CategoryDTO> ToCategoryDTO(List<Category> categories)
        {
            return categories.Select(c => new CategoryDTO
            {
                Libelle = c.Libelle
            }).ToList();
        }

        public static CategoryDTO ToCategoryDTO(Category category)
        {
            return new CategoryDTO
            {
                Libelle = category.Libelle
            };
        }

        public static Category ToCategory(CategoryDTO category)
        {
            return new Category
            {
                Libelle = category.Libelle
            };
        }
    }
}
