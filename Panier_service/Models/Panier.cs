using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Panier_service.Models
{
    public class Panier
    {
        public int Id_client { get; set; }
        public int Id_produit { get; set; }
        public int Qunatite { get; set; }
        public double? Prix { get; set; }

    }
}
