using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Help;
using WebApplication2.Models.Repositories;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class PanierController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IDeliveryMethodRepository deliveryRepository;
        private readonly UserManager<IdentityUser> userManager;

        public PanierController(IProductRepository productRepository, IOrderRepository orderRepository,
            IDeliveryMethodRepository deliveryRepository, UserManager<IdentityUser> userManager)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.deliveryRepository = deliveryRepository;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.Liste = ListeCart.Instance.Items;
            ViewBag.Total = ListeCart.Instance.GetSubTotal();
            return View(ListeCart.Instance.Items);
        }

        public IActionResult AddProduct(int id)
        {
            Product? product = productRepository.GetById(id);
            if (product == null) return NotFound();
            int current = ListeCart.Instance.Items.FirstOrDefault(i => i.Prod.ProductId == id)?.quantite ?? 0;
            if (current + 1 > product.QteStock)
            {
                TempData["Erreur"] = $"Stock insuffisant pour {product.Name} (disponible : {product.QteStock}).";
                return RedirectToAction("Index", "Product");
            }
            ListeCart.Instance.AddItem(product);
            TempData["Info"] = $"{product.Name} a été ajouté au panier.";
            return RedirectToAction(nameof(Index));
        }

        // Routes utilisées par la vue classique du panier.
        [HttpGet]
        public IActionResult IncreaseProduct(int id) => AddProduct(id);

        [HttpGet]
        public IActionResult DecreaseProduct(int id)
        {
            ListeCart.Instance.DecreaseItem(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult PlusProduct(int id)
        {
            Product? product = productRepository.GetById(id);
            if (product == null) return NotFound();
            Item? item = ListeCart.Instance.Items.FirstOrDefault(i => i.Prod.ProductId == id);
            if (item != null && item.quantite + 1 > product.QteStock)
            {
                TempData["Erreur"] = $"Stock insuffisant pour {product.Name} (disponible : {product.QteStock}).";
                return Json(new { ct = 0 });
            }
            ListeCart.Instance.AddItem(product);
            item = ListeCart.Instance.Items.First(i => i.Prod.ProductId == id);
            return Json(new { ct = 1, Total = ListeCart.Instance.GetSubTotal(), Quatite = item.quantite, TotalRow = item.TotalPrice });
        }

        [HttpPost]
        public IActionResult MinusProduct(int id)
        {
            ListeCart.Instance.DecreaseItem(id);
            Item? item = ListeCart.Instance.Items.FirstOrDefault(i => i.Prod.ProductId == id);
            return Json(new { ct = item == null ? 0 : 1, Total = ListeCart.Instance.GetSubTotal(), Quatite = item?.quantite ?? 0, TotalRow = item?.TotalPrice ?? 0 });
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult RemoveProduct(int id)
        {
            ListeCart.Instance.RemoveItem(id);
            if (Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction(nameof(Index));
            return Json(new { Total = ListeCart.Instance.GetSubTotal() });
        }

        private List<CartItemViewModel> CartLines() => ListeCart.Instance.Items.Select(item => new CartItemViewModel
        {
            ProductName = item.Prod.Name, Quantity = item.quantite, Price = item.Prod.Price
        }).ToList();

        private void PrepareDisplay(OrderViewModel model)
        {
            model.CartItems = CartLines();
            model.DeliveryMethods = deliveryRepository.GetAll();
            model.SubTotal = ListeCart.Instance.GetSubTotal();
            DeliveryMethod? delivery = model.DeliveryMethodId.HasValue ? deliveryRepository.GetById(model.DeliveryMethodId.Value) : null;
            model.ShippingPrice = delivery?.Prix ?? 0;
            model.TotalAmount = model.SubTotal + model.ShippingPrice;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!ListeCart.Instance.Items.Any()) return RedirectToAction(nameof(Index));
            var model = new OrderViewModel();
            PrepareDisplay(model);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderViewModel model)
        {
            if (!ListeCart.Instance.Items.Any()) return RedirectToAction(nameof(Index));
            DeliveryMethod? delivery = model.DeliveryMethodId.HasValue ? deliveryRepository.GetById(model.DeliveryMethodId.Value) : null;
            if (delivery == null) ModelState.AddModelError(nameof(model.DeliveryMethodId), "Choisissez un mode de livraison");
            foreach (Item item in ListeCart.Instance.Items)
            {
                Product? product = productRepository.GetById(item.Prod.ProductId);
                if (product == null || product.QteStock < item.quantite)
                    ModelState.AddModelError("", $"Stock insuffisant pour {item.Prod.Name} (disponible : {product?.QteStock ?? 0}).");
            }
            if (!ModelState.IsValid)
            {
                PrepareDisplay(model);
                return View(model);
            }
            IdentityUser? user = await userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            float subtotal = ListeCart.Instance.GetSubTotal();
            var order = new Order
            {
                CustomerName = user.UserName ?? user.Email ?? "",
                Email = user.Email ?? "",
                FirstName = model.FirstName, LastName = model.LastName, Address = model.Address,
                City = model.City, ZipCode = model.ZipCode, Phone = model.Phone,
                SubTotal = subtotal, ShippingPrice = delivery!.Prix, TotalAmount = subtotal + delivery.Prix,
                DeliveryMethodId = delivery.Id, PaymentMethod = model.PaymentMethod,
                Status = OrderStatus.EnAttente, OrderDate = DateTime.Now, UserId = user.Id,
                Items = ListeCart.Instance.Items.Select(item => new OrderItem
                {
                    ProductId = item.Prod.ProductId, ProductName = item.Prod.Name,
                    Quantity = item.quantite, Price = item.Prod.Price
                }).ToList()
            };
            orderRepository.Add(order);
            foreach (Item item in ListeCart.Instance.Items) productRepository.UpdateStock(item.Prod.ProductId, -item.quantite);
            ListeCart.Instance.Clear();
            TempData["SuccessMessage"] = "Votre commande a été passée avec succès.";
            return RedirectToAction(nameof(Confirmation), new { orderId = order.Id });
        }

        [Authorize]
        public IActionResult Confirmation(int orderId)
        {
            Order? order = orderRepository.GetById(orderId);
            return order == null || order.UserId != userManager.GetUserId(User) ? NotFound() : View(order);
        }
    }
}
