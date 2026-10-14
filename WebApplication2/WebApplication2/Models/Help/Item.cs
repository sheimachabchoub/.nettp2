using WebApplication2.Models;

namespace WebApplication2.Models.Help
{
    public class Item
    {
        public int quantite { get; set; }

        private Product? _product;

        public Product Prod
        {
            get => _product;
            set => _product = value;
        }

        public string Description => _product?.Name ?? string.Empty;
        public float UnitPrice => _product?.Price ?? 0;
        public int categoryId => _product?.CategoryId ?? 0;
        public float TotalPrice => UnitPrice * quantite;
    }
}
