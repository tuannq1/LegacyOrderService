using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LegacyOrderService.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = [];
    }
}
