using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public string Phone { get; set; } = "";
        public float SubTotal { get; set; }
        public float ShippingPrice { get; set; }
        public float TotalAmount { get; set; }
        public int? DeliveryMethodId { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string Status { get; set; } = OrderStatus.EnAttente;
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; } = "";
        public IdentityUser? User { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
