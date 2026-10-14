using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;
        public OrderRepository(AppDbContext context) => this.context = context;
        public void Add(Order order) { context.Orders.Add(order); context.SaveChanges(); }
        public void Update(Order order) { context.Orders.Update(order); context.SaveChanges(); }
        public Order? GetById(int id) => context.Orders.Include(o => o.Items).Include(o => o.DeliveryMethod).FirstOrDefault(o => o.Id == id);
        public List<Order> GetByUserId(string userId) => context.Orders.Include(o => o.DeliveryMethod).Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList();
        public List<Order> GetAll() => context.Orders.Include(o => o.DeliveryMethod).OrderByDescending(o => o.OrderDate).ToList();
    }
}
