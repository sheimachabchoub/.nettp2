using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IOrderRepository
    {
        Order? GetById(int id);
        void Add(Order order);
        void Update(Order order);
        List<Order> GetByUserId(string userId);
        List<Order> GetAll();
    }
}
