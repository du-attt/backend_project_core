using backend_project_core.Data;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly MyDbContext _myDbContext; // Tạo kết nối đến database

        public CategorieController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _myDbContext.Categories.ToListAsync();
                return Ok(new { categories });
                //return Ok(categories);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        [Route("categories/{id}")]
        public async Task<ActionResult<Categories>> GetCategoriesID(string id)
        {
            // Lấy sản phẩm theo id
            var categorie = await _myDbContext.Categories.FirstOrDefaultAsync(p => p._id.Equals(id));
            if(categorie == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(categorie);
            }
        }


    }
}
