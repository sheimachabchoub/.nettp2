using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        readonly ICategorieRepository CategRepository;

        public CategoryController(ICategorieRepository categRepository)
        {
            CategRepository = categRepository;
        }

        // GET: CategoryController
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewData["Categories"] = CategRepository.GetAll();
            var Categories = CategRepository.GetAll();
            return View(Categories);
        }

        // GET: CategoryController/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll();
            var category = CategRepository.GetById(id);
            return View(category);
        }

        // GET: CategoryController/Create
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            ViewData["Categories"] = CategRepository.GetAll();
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(Category c)
        {
            try
            {
                CategRepository.Add(c);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll();
            var category = CategRepository.GetById(id);
            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int id, Category c)
        {
            try
            {
                CategRepository.Update(c);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll();
            var category = CategRepository.GetById(id);
            return View(category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                CategRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
