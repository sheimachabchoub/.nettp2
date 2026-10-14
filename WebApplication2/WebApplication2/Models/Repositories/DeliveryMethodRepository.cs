using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class DeliveryMethodRepository : IDeliveryMethodRepository
    {
        private readonly AppDbContext context;
        public DeliveryMethodRepository(AppDbContext context) => this.context = context;
        public List<DeliveryMethod> GetAll() => context.DeliveryMethods.OrderByDescending(d => d.Prix).ToList();
        public DeliveryMethod? GetById(int id) => context.DeliveryMethods.Find(id);
    }
}
