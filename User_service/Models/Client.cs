using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using User_service.Enums;

namespace User_service.Models
{
    
    [Index(nameof(UserId), IsUnique = true)]
    public class Client
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
