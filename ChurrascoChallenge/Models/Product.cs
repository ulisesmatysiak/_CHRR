using System.ComponentModel.DataAnnotations;

namespace ChurrascoChallenge.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        public int SKU { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Picture URL is required.")]
        public string Picture { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        public string Currency { get; set; }

        public int? Code { get; set; }

        public string? Description { get; set; } 
    }
}
