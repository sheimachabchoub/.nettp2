using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Le prénom est obligatoire")] public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Le nom est obligatoire")] public string LastName { get; set; } = "";
        [Required(ErrorMessage = "La rue est obligatoire")] public string Address { get; set; } = "";
        [Required(ErrorMessage = "La ville est obligatoire")] public string City { get; set; } = "";
        [Required(ErrorMessage = "Le code postal est obligatoire")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Le code postal doit contenir 4 chiffres")] public string ZipCode { get; set; } = "";
        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Le téléphone doit contenir 8 chiffres")] public string Phone { get; set; } = "";
        [Required(ErrorMessage = "Choisissez un mode de livraison")] public int? DeliveryMethodId { get; set; }
        [Required(ErrorMessage = "Choisissez un mode de paiement")] public string PaymentMethod { get; set; } = "Carte";
        public float SubTotal { get; set; }
        public float ShippingPrice { get; set; }
        public float TotalAmount { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new();
        public List<DeliveryMethod> DeliveryMethods { get; set; } = new();
    }
}
