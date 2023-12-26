using backend_project_core.Data;
//using backend_project_core.Migrations;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using Azure.Core;
using Newtonsoft.Json;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public CartController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        // Tạo chi tiết giỏ hàng khi thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [Route("user/cart")]
        public IActionResult CreateCartDetail([FromBody] CartDetailModel model)
        {

            var product = model.product;
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            Console.WriteLine($"Token Value: {tokenValue}");


            if (string.IsNullOrEmpty(tokenValue))
            {
                // Token không tồn tại hoặc không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid tokensss",
                });
            }

            // Phân tích JWT để lấy thông tin
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken; 

            if (token == null)
            {
                // Token không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token format",
                });
            }

            //Lấy thông tin từ JWT
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {
                    var existingCart = _myDbContext.Carts.SingleOrDefault(p => p.idUser == id);
                    if (existingCart != null)
                    {
                        var existingCartDetail = _myDbContext.CartDetails.SingleOrDefault(p => p.idCart == existingCart.id && p._idProduct == product._id);
                        if (existingCartDetail == null)
                        {
                            var CartDetail = new CartDetails
                            {
                                idCart = existingCart.id,
                                _idProduct = product._id,
                                qty = 1
                                
                            };
                            _myDbContext.CartDetails.Add(CartDetail);
                            _myDbContext.SaveChanges();
                        }

                        else
                        {
                            existingCartDetail.qty += 1;
                            _myDbContext.Entry(existingCartDetail).State = EntityState.Modified;
                            _myDbContext.SaveChanges();
                        }

                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Authenticate success",
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
        

        // Lấy các chi tiết giỏ hàng từ trong giỏ hàng của người dùng
        [HttpGet]
        [Route("user/cart")]
        public async Task<IActionResult> GetCart()
        {
            var tokenValue = Request.Headers["Authorization"];
            
            if (string.IsNullOrEmpty(tokenValue))
            {
                // Token không tồn tại hoặc không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid tokennn",
                });
            }

            // Phân tích JWT để lấy thông tin
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;

            if (token == null)
            {
                // Token không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token format",
                });
            }

            //Lấy thông tin từ JWT
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {

                    // Lấy giỏ hàng thuộc user
                    var existingCart = _myDbContext.Carts.SingleOrDefault(p => p.idUser == id);
                    if (existingCart != null)
                    {
                        // Lấy chi tiết giỏ hàng thuộc giỏ hàng
                        var existingCartDetails = _myDbContext.CartDetails.Where(p => p.idCart == existingCart.id).ToList();

                        if (existingCartDetails != null)
                        {
                            // Lấy ra sản phẩm thuộc chi tiết giỏ hàng
                            var productIds = existingCartDetails.Select(cd => cd._idProduct).ToList();

                            var products = (from p in _myDbContext.Products
                                            join cd in _myDbContext.CartDetails on p._id equals cd._idProduct
                                            where cd.idCart == existingCart.id
                                            select new
                                {
                                    _id = p._id,
                                    qty = cd.qty,
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
                                })
                                .ToList();

                            return Ok(new { cart = products });
                        }
                        else
                        {
                            // Trường hợp giỏ hàng rỗng
                            return Ok(new { cart = new List<object>() });
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

        // Cập nhật số lượng sản phẩm trong chi tiết giỏ hàng
        [HttpPost]
        [Route("user/cart/{ProductID}")]
        public IActionResult UpdateProductQtyCart(string ProductID, [FromBody] CartUpdateRequest request)
        {

            var type = request.Action.Type;

            var tokenValue = HttpContext.Request.Headers["Authorization"];


            if (string.IsNullOrEmpty(tokenValue))
            {
                // Token không tồn tại hoặc không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid tokensss",
                });
            }

            // Phân tích JWT để lấy thông tin
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;

            if (token == null)
            {
                // Token không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token format",
                });
            }

            //Lấy thông tin từ JWT
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {

                    // Lấy giỏ hàng thuộc user
                    var existingCart = _myDbContext.Carts.SingleOrDefault(p => p.idUser == id);

                    if (existingCart != null)
                    {
                        // Lấy chi tiết giỏ hàng thuộc giỏ hàng
                        var existingCartDetails = _myDbContext.CartDetails.Where(p => p.idCart == existingCart.id).ToList();
                        foreach (var cartDetail in existingCartDetails)
                        {
                            if (cartDetail._idProduct == ProductID)
                            {
                                if (type == "increment")
                                {
                                    cartDetail.qty += 1;
                                    _myDbContext.Entry(cartDetail).State = EntityState.Modified;
                                    _myDbContext.SaveChanges();
                                    return Ok();
                                }
                                else
                                {
                                    cartDetail.qty -= 1;
                                    _myDbContext.Entry(cartDetail).State = EntityState.Modified;
                                    _myDbContext.SaveChanges();
                                    return Ok();
                                }
                            }
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
        public class CartUpdateRequest
        {
            public CartActionModel Action { get; set; }
        }
        public class CartActionModel
        {
            public string Type { get; set; }
        }


        // Xóa chi tiết giỏ hàng
        [HttpDelete]
        [Route("user/cart/{ProductID}")]
        public IActionResult DeleteProductFromCart(string ProductID)
        {
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            Console.WriteLine(tokenValue);
            if (string.IsNullOrEmpty(tokenValue))
            {
                // Token không tồn tại hoặc không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid tokensss",
                });
            }

            // Phân tích JWT để lấy thông tin
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
            if (token == null)
            {
                // Token không hợp lệ
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token format",
                });
            }

            //Lấy thông tin từ JWT
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            Console.WriteLine(userId);
            if (userId != null)
            {

                if (int.TryParse(userId, out int id))
                {
                    // Lấy giỏ hàng thuộc user
                    var existingCart = _myDbContext.Carts.SingleOrDefault(p => p.idUser == id);

                    if (existingCart != null)
                    {
                        // Lấy chi tiết giỏ hàng thuộc giỏ hàng
                        var existingCartDetails = _myDbContext.CartDetails.Where(p => p.idCart == existingCart.id).ToList();
                        foreach (var cartDetail in existingCartDetails)
                        {
                            if (cartDetail._idProduct == ProductID)
                            {
                                _myDbContext.CartDetails.Remove(cartDetail);
                                _myDbContext.SaveChanges();
                                var products = (from p in _myDbContext.Products
                                                join cd in _myDbContext.CartDetails on p._id equals cd._idProduct
                                                where cd.idCart == existingCart.id
                                                select new
                                                {
                                                    _id = p._id,
                                                    qty = cd.qty,
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

                                return Ok(new { cart = products });
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
