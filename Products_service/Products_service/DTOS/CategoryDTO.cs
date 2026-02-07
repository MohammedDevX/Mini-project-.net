using System.ComponentModel.DataAnnotations;

namespace Products_service.DTOS
{
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Le libelle est obligatoire")]
        public string Libelle { get; set; } = null!;
    }
}
