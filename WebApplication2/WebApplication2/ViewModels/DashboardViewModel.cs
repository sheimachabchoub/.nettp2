using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class DashboardViewModel
    {
        public int NbProduits { get; set; }
        public int NbCommandes { get; set; }
        public int NbEnAttente { get; set; }
        public float ChiffreAffaires { get; set; }
        public List<Product> ProduitsEnRupture { get; set; } = new();
        public List<Order> DernieresCommandes { get; set; } = new();
    }
}
