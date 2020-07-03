using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController: CustomController<Category, CategoryService>
    {
        public readonly CategoryService service;

        public CategoryController(CategoryService _service, ILogger<CategoryController> logger) : base(_service, logger)
        {
            this.service = _service;
        }
    }
}
