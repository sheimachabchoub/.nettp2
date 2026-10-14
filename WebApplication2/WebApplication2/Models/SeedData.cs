using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            foreach (string role in new[] { "Admin", "Manager", "User" })
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

            // Comptes administrateurs de démonstration. Si un compte existe déjà,
            // son rôle Admin est réappliqué sans modifier son mot de passe.
            foreach (string email in new[] { "admin@shop.com", "admin@gmail.com" })
            {
                IdentityUser? admin = await userManager.FindByEmailAsync(email);
                if (admin == null)
                {
                    admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(admin, "admin2026");
                }

                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Un seul mode de livraison pour la démonstration : 7 DT.
            List<DeliveryMethod> deliveryMethods = await context.DeliveryMethods
                .OrderBy(d => d.Id)
                .ToListAsync();

            DeliveryMethod delivery;
            if (deliveryMethods.Count == 0)
            {
                delivery = new DeliveryMethod
                {
                    Nom = "Livraison standard",
                    Delai = "3 à 5 jours",
                    Prix = 7
                };
                context.DeliveryMethods.Add(delivery);
                await context.SaveChangesAsync();
            }
            else
            {
                delivery = deliveryMethods[0];
                delivery.Nom = "Livraison standard";
                delivery.Delai = "3 à 5 jours";
                delivery.Prix = 7;

                // Les anciennes commandes gardent une référence valide avant
                // la suppression des anciens modes de livraison.
                List<Order> orders = await context.Orders.ToListAsync();
                foreach (Order order in orders)
                {
                    if (order.DeliveryMethodId.HasValue && order.DeliveryMethodId != delivery.Id)
                        order.DeliveryMethodId = delivery.Id;
                }

                if (deliveryMethods.Count > 1)
                    context.DeliveryMethods.RemoveRange(deliveryMethods.Skip(1));

                await context.SaveChangesAsync();
            }
        }
    }
}
