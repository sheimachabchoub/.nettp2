using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<IdentityUser> userManager;

        public OrderController(IOrderRepository orderRepository, IProductRepository productRepository,
            UserManager<IdentityUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }

        public IActionResult MesCommandes() => View(orderRepository.GetByUserId(userManager.GetUserId(User) ?? ""));

        public IActionResult Details(int id)
        {
            Order? order = orderRepository.GetById(id);
            if (order == null || (order.UserId != userManager.GetUserId(User) && !User.IsInRole("Admin"))) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Annuler(int id)
        {
            Order? order = orderRepository.GetById(id);
            if (order == null || order.UserId != userManager.GetUserId(User)) return NotFound();
            if (order.Status != OrderStatus.EnAttente)
            {
                TempData["Erreur"] = "Seule une commande en attente peut être annulée.";
                return RedirectToAction(nameof(MesCommandes));
            }
            order.Status = OrderStatus.Annulee;
            orderRepository.Update(order);
            RestoreStock(order);
            TempData["Info"] = $"La commande n°{order.Id} a été annulée.";
            return RedirectToAction(nameof(MesCommandes));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Gestion(string? statut)
        {
            var orders = orderRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(statut)) orders = orders.Where(o => o.Status == statut).ToList();
            ViewBag.Statut = statut;
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangerStatut(int id, string statut)
        {
            Order? order = orderRepository.GetById(id);
            if (order == null) return NotFound();
            if (!OrderStatus.Tous.Contains(statut)) return BadRequest();
            if (order.Status == OrderStatus.Annulee)
            {
                TempData["Erreur"] = "Une commande annulée ne peut plus être modifiée.";
                return RedirectToAction(nameof(Gestion));
            }
            if (statut == OrderStatus.Annulee) RestoreStock(order);
            order.Status = statut;
            orderRepository.Update(order);
            TempData["Info"] = $"Commande n°{id} : statut changé en « {statut} ».";
            return RedirectToAction(nameof(Gestion));
        }

        private void RestoreStock(Order order)
        {
            foreach (OrderItem item in order.Items)
                if (item.ProductId.HasValue) productRepository.UpdateStock(item.ProductId.Value, item.Quantity);
        }
    }
}
