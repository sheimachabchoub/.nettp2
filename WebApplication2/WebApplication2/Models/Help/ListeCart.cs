namespace WebApplication2.Models.Help
{
    public sealed class ListeCart
    {
        private static readonly ListeCart instance = new ListeCart();

        private ListeCart()
        {
        }

        public static ListeCart Instance => instance;

        public List<Item> Items { get; } = new List<Item>();

        public void AddItem(Product product)
        {
            Item item = Items.FirstOrDefault(i => i.Prod.ProductId == product.ProductId);
            if (item == null)
            {
                Items.Add(new Item { Prod = product, quantite = 1 });
            }
            else
            {
                item.quantite++;
            }
        }

        public void DecreaseItem(int productId)
        {
            Item item = Items.FirstOrDefault(i => i.Prod.ProductId == productId);
            if (item == null)
            {
                return;
            }

            item.quantite--;
            if (item.quantite <= 0)
            {
                Items.Remove(item);
            }
        }

        public void RemoveItem(int productId)
        {
            Item item = Items.FirstOrDefault(i => i.Prod.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void Clear() => Items.Clear();

        public float GetSubTotal() => Items.Sum(i => i.TotalPrice);
    }
}
