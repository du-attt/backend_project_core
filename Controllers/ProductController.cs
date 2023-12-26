using backend_project_core.Data;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyDbContext _myDbContext; //Tạo kết nối với database

        public ProductController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }


        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _myDbContext.Products.ToListAsync();
                
                return Ok(new { products });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }
        [HttpGet]
        [Route("products/{id}")]
        public async Task<ActionResult<Products>> GetProductById(string id)
        {
            // Lấy sản phẩm theo id
            var product = await _myDbContext.Products.FirstOrDefaultAsync(p => p._id.Equals(id));

            // Kiểm tra xem sản phẩm có tồn tại hay không
            if (product == null)
            {
                // Trả về NotFound nếu không tìm thấy sản phẩm
                return NotFound("Product not found");
            }
            // Trả về sản phẩm tìm thấy
            return product;
        }


    }
}
