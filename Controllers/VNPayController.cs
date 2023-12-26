using backend_project_core.Data;
using backend_project_core.VNPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using log4net;
using System.Net;
using Microsoft.EntityFrameworkCore;
using backend_project_core.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Primitives;
using backend_project_core.Migrations;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MyDbContext _myDbContext;

        public VNPayController(IConfiguration configuration, MyDbContext myDbContext)
        {
            _configuration = configuration;
            _myDbContext = myDbContext;
        }
      


        [HttpPost]
        [Route("vnpay")]
        public IActionResult VNPay([FromBody] TotalRequest request)
        {

            string vnp_Returnurl = "https://localhost:7060/api/callback"; // Đặt URL callback của bạn
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL thanh toán của VNPay
            string vnp_TmnCode = _configuration["AppSettings:VnPay_TmnCode"]; // Lấy mã website từ cấu hình
            string vnp_HashSecret = _configuration["AppSettings:VnPay_HashSecret"]; // Lấy chuỗi bí mật từ cấu hình


            var tokenValue = HttpContext.Request.Headers["Authorization"];
            // Đọc token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
            //Lấy thông tin user từ JWT token
            var userId = token.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            //Get payment input
            OrderInfo order = new OrderInfo();
            //Save order to db

            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = request.Action.Type; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            order.OrderDesc = "Nhập nội dung thanh toán";
            order.CreatedDate = DateTime.Now;
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100000).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            var clientIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            vnpay.AddRequestData("vnp_IpAddr", clientIp);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "Mua sắm"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString() + userId); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
           // vnpay.AddRequestData("vnp_id", order.userId);
            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new { paymentUrl });

        }
        public class TotalRequest
        {
            public ActionModel Action { get; set; }
        }
        public class ActionModel
        {
            public long Type { get; set; }
        }
        



        [HttpGet]
        [Route("callback")]
        public IActionResult StatusAsync()
        {
            
            if (Request.QueryString.Value.Length > 0)
            {
                string vnp_HashSecret = _configuration["AppSettings:VnPay_HashSecret"]; // Lấy chuỗi bí mật từ cấu hình
                var vnpayData = Request.Query;

                //return Json(vnpayData);
                VnPayLibrary vnpay = new VnPayLibrary();
                if (vnpayData.Count > 0)
                {
                    foreach (var s in vnpayData)
                    {
                        //get all querystring data
                        if (!string.IsNullOrEmpty(s.Key) && s.Key.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(s.Key, s.Value);
                        }
                    }
                }
                //Lay danh sach tham so tra ve tu VNPAY
                string orderId = vnpay.GetResponseData("vnp_TxnRef");
                var userId = orderId[orderId.Length - 1].ToString();

                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                //vnp_SecureHash: MD5 cua du lieu tra ve
                string vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {

                    if (vnp_ResponseCode == "00")
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
                                PaymentMethod = "Thanh toán bằng VNPay",
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
                                foreach (var cartDetail in existingCartDetails)
                                {
                                    _myDbContext.CartDetails.Remove(cartDetail);
                                }
                                _myDbContext.Invoice.Add(invoice);
                                _myDbContext.SaveChanges();

                            }


                        }

                            //Thanh toan thanh cong
                            return new ContentResult
                        {
                            Content = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                            "<title>Green Div with Image</title></head><body style='display: flex; justify-content: center; align-items: center; text-align: center; height: 100vh; margin: 0;'>" +
                            "<div><img src='https://lh3.googleusercontent.com/E3oVhQdQLAzUsho5_jfUj1-v15ZqYbetuLUEAVRVhPtv949bAgivtclGP4HmwM75T6YTaied1BlAV8l5RXQovyKcdrlef_7Ruoy2rZW8pNArCCjWr_q0kSOT5yHhx_gnFokiuU9mXR0j62DoNTTQKGM' alt='' /></div>" +
                            "</body></html>",
                            ContentType = "text/html; charset=utf-8",
                            StatusCode = 200
                        };

                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        return new ContentResult
                        {
                            Content = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                            "<title>Green Div with Image</title></head><body style='display: flex; justify-content: center; align-items: center; text-align: center; height: 100vh; margin: 0;'>" +
                            "<div><img src='https://nxbgdhcm.vn/wp-content/uploads/2022/08/thongbao.jpg' alt=''><h3 style='color: red'> Thanh toán không thành công </h3></div>" +
                            "</body></html>",
                            ContentType = "text/html;  charset=utf-8",
                            StatusCode = 201
                        };
                    }
                }
                else
                {
                    return new ContentResult
                    {
                        Content = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                             "<title>Green Div with Image</title></head><body style='display: flex; justify-content: center; align-items: center; text-align: center; height: 100vh; margin: 0;'>" +
                             "<div><img src='https://nxbgdhcm.vn/wp-content/uploads/2022/08/thongbao.jpg' alt=''><h3 style='color: red'> Thanh toán không thành công </h3></div>" +
                             "</body></html>",
                        ContentType = "text/html;  charset=utf-8",
                        StatusCode = 201
                    };
                }
            }
            else
            {
                return new ContentResult
                {
                   Content = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                             "<title>Green Div with Image</title></head><body style='display: flex; justify-content: center; align-items: center; text-align: center; height: 100vh; margin: 0;'>" +
                             "<div><img src='https://nxbgdhcm.vn/wp-content/uploads/2022/08/thongbao.jpg' alt=''><h3 style='color: red'> Thanh toán không thành công </h3></div>" +
                             "</body></html>",
                        ContentType = "text/html;  charset=utf-8",
                        StatusCode = 201
                };
            }
        }
        
    }
}
