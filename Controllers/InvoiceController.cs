using backend_project_core.Data;
using backend_project_core.Migrations;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;
        public InvoiceController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        // Tạo hóa đơn
        [HttpPost]
        [Route("invoice")]
        public IActionResult createInvoice([FromBody] InvoiceRequest request)
        {
            var tokenValue = HttpContext.Request.Headers["Authorization"];
            var data = request.Action.Type;
            Console.WriteLine($"data Value: {data}");
            Console.WriteLine($"Token Value: {tokenValue}");

            string PhuongThuc = "";
            if (data == "pay")
            {
                PhuongThuc = "Thanh toán khi nhận hàng";
            }
            else if (data == "VNPay")
            {
                PhuongThuc = "Thanh toán bằng VNPay";
            }
            else
            {
                PhuongThuc = "Thanh toán bằng Momo";

            }

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
                    // lấy thông tin User
                    var existingUser = _myDbContext.Users.SingleOrDefault(p => p.id == id);

                    // Tạo invoice
                    var invoice = new Invoice
                    {
                        idUser = id,
                        Company = "MATVIET",
                        ShippingFee = 0,
                        PaymentMethod = PhuongThuc,
                        CustomerName = existingUser.name,
                        CustomerAddress = "NT",
                        IssueDate = DateTime.Now,

                     };

                    // Lấy giỏ hàng thuộc user
                    var existingCart = _myDbContext.Carts.SingleOrDefault(p => p.idUser == id);
                    if (existingCart != null)
                    {
                        // Lấy chi tiết giỏ hàng thuộc giỏ hàng
                        var existingCartDetails = _myDbContext.CartDetails.Where(p => p.idCart == existingCart.id).ToList();

                        foreach (var cardetail in existingCartDetails)
                        {
                            var product = _myDbContext.Products.SingleOrDefault(p => p._id == cardetail._idProduct);
                            invoice.TotalAmount += product.price * cardetail.qty;
                            invoice.GrandTotal += product.newPrice * cardetail.qty;

                            // Tạo mới một chi tiết hóa đơn
                            var invoiceDetail = new InvoiceDetail
                            {
                                idInvoice = invoice.Id,
                                _idProduct = product._id,
                                qty = cardetail.qty,
                            };
                            _myDbContext.InvoiceDetail.Add(invoiceDetail);


                        }
                        _myDbContext.Invoice.Add(invoice);
                        _myDbContext.SaveChanges();

                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "success",
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
        public class InvoiceRequest
        {
            public InvoiceActionModel Action { get; set; }
        }
        public class InvoiceActionModel
        {
            public string Type { get; set; }
        }
        // Lấy hóa đơn
        [HttpGet]
        [Route("invoice")]
        public async Task<IActionResult> GetInvoice()
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

                    // Lấy hóa đơn
                    var Invoice = _myDbContext.Invoice.Where(p => p.idUser == id).ToList();
                    return Ok(new {Invoice});

                }

            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid token format",
            });
        }
        [HttpGet]
        [Route("invoice/{idInvoice}")]
        public async Task<IActionResult> GetInvoiceDetail(string idInvoice)
        {
            var InvoiceDetails = _myDbContext.InvoiceDetail.Where(p => p.idInvoice == idInvoice).ToList();
            if(InvoiceDetails != null)
            {
                var products = (from cd in InvoiceDetails
                                join p in _myDbContext.Products on cd._idProduct equals p._id
                                select new
                                {
                                    _id = p._id,
                                    name = p.name,
                                    image = p.image,
                                    price = p.price,
                                    newPrice = p.newPrice,
                                    qty = cd.qty,
                                }).ToList();

                return Ok(new { items = products });
            }
              
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid token format",
            });
        }
    }
}
