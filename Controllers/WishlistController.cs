using backend_project_core.Data;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly MyDbContext _myDbContext ;
        public WishlistController(MyDbContext myDbContext) {
            _myDbContext = myDbContext;
        }
        [HttpPost]
        [Route("user/wishlist")]
        public IActionResult AddProductToWishlistService([FromBody] CartDetailModel model)
        {
            // Lấy sản phẩm và token
            var product = model.product;
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            // Đọc token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
            //Lấy thông tin user từ JWT token
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;


            if (userId != null)
            {
                // Chuyển đổi kiểu dữ liệu của user
                if (int.TryParse(userId, out int id))
                {
                    // Lấy wishlish ứng với user
                    var existingWishlish = _myDbContext.Wishlists.SingleOrDefault(p => p.idUser == id);
                    if (existingWishlish != null)
                    {
                        // Kiểm tra wishlishDetails trong wishlish đã tồn tại chưa
                        var existingWishlishDetail = _myDbContext.WishlishDetails.SingleOrDefault(p => p.idWishlish == existingWishlish.id && p._idProduct == product._id);
                        
                        if (existingWishlishDetail == null)
                        {
                            var WishlishDetails = new WishlishDetails
                            {
                                idWishlish = existingWishlish.id,
                                _idProduct = product._id,
                            };
                            _myDbContext.WishlishDetails.Add(WishlishDetails);
                            _myDbContext.SaveChanges();
                        }
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "success",
                            access_token = token,
                        });
                    }
                }
            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid token format",
            });
        }

        [HttpGet]
        [Route("user/wishlist")]
        public IActionResult GetWishlistItemsService()
        {
            // Lấy token
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            // Đọc token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
            //Lấy thông tin user từ JWT token
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {

                    // Lấy wishlish thuộc user
                    var existingWishlish = _myDbContext.Wishlists.SingleOrDefault(p => p.idUser == id);
                    if (existingWishlish != null)
                    {
                        // Lấy wishlishDetails thuộc wishlish
                        var existingWishlishDetails= _myDbContext.WishlishDetails.Where(p => p.idWishlish == existingWishlish.id).ToList();

                        if (existingWishlishDetails != null)
                        {
                            // Lấy ra sản phẩm thuộc wishlishDetails
                            var productIds = existingWishlishDetails.Select(cd => cd._idProduct).ToList();

                            var products = _myDbContext.Products
                                .Where(p => productIds.Contains(p._id))
                                .Select(p => new
                                {
                                    _id = p._id,
                                    qty = p.qty,
                                    name = p.name,
                                    description = p.description,
                                    brand = p.brand,
                                    category = p.category,
                                    gender = p.gender,
                                    weight = p.weight,
                                    quantity = p.quantity,
                                    image = p.image,
                                    rating = p.rating,
                                    price = p.price,
                                    newPrice = p.newPrice,
                                    trending = p.trending
                                }).ToList();

                            return Ok(new { wishlist = products });
                        }
                        else
                        {
                            // Trường hợp wishlish rỗng
                            return Ok(new { wishlist = new List<object>() });
                        }


                    }


                }
            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid token format",
            });
        }

        [HttpDelete]
        [Route("user/wishlist/{ProductID}")]
        public IActionResult DeleteProductFromWishlistService(string ProductID) 
        {
            // Lấy token
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            // Đọc token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
            //Lấy thông tin user từ JWT token
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {
                    // Lấy wishlish thuộc user
                    var existingWishlish = _myDbContext.Wishlists.SingleOrDefault(p => p.idUser == id);
                    if (existingWishlish != null)
                    {
                        // Lấy wishlishDetails thuộc wishlish
                        var existingWishlishDetails = _myDbContext.WishlishDetails.Where(p => p.idWishlish == existingWishlish.id).ToList();
                        foreach(var wishlishDetail in existingWishlishDetails)
                        {
                            if (wishlishDetail._idProduct == ProductID)
                            {
                                _myDbContext.WishlishDetails.Remove(wishlishDetail);
                                _myDbContext.SaveChanges();
                                var products = (from p in _myDbContext.Products
                                                select new
                                                {
                                                    _id = p._id,
                                                    qty = p.qty,
                                                    name = p.name,
                                                    description = p.description,
                                                    brand = p.brand,
                                                    category = p.category,
                                                    gender = p.gender,
                                                    weight = p.weight,
                                                    quantity = p.quantity,
                                                    image = p.image,
                                                    rating = p.rating,
                                                    price = p.price,
                                                    newPrice = p.newPrice,
                                                    trending = p.trending,
                                                    inWish = true
                                                }).ToList();

                                return Ok(new { wishlist = products });
                            }
                        }

                    }

                }
            }

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid",
            });
        }
    }
}

