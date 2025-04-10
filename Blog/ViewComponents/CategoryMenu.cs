using Blog.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Blog.ViewComponents
{
    public class CategoryMenu : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryMenu(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _categoryRepository.Categories.ToList();
            return View(categories);
        }
    }
}
