using Blog.Data.Abstract;
using Blog.Entity;
using Blog.Entity.Blog.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Helpers;
using Blog.Data.Concrete.EfCore;

namespace Blog.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                model.Url = model.Name.ToSeoUrl();


                var context = (_categoryRepository as BlogAppContext) ?? throw ExceptionHelper.EfContextNotFound();
                context.Categories.Add(model);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                model.Url = model.Name.ToSeoUrl();


                var context = (_categoryRepository as BlogAppContext) ?? throw ExceptionHelper.EfContextNotFound();
                context.Categories.Update(model);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }


        public IActionResult Delete(int id)
        {
            var context = (_categoryRepository as BlogAppContext) ?? throw ExceptionHelper.EfContextNotFound();
            var category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
