using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IDeliveryMethodRepository
    {
        List<DeliveryMethod> GetAll();
        DeliveryMethod? GetById(int id);
    }
}
