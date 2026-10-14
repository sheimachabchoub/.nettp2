namespace WebApplication2.Models
{
    public class DeliveryMethod
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Delai { get; set; } = "";
        public float Prix { get; set; }
    }
}
