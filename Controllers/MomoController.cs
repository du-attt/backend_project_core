using backend_project_core.Momo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static backend_project_core.Controllers.VNPayController;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        [HttpGet]
        [Route("momo")]
        public IActionResult MomoPaymentUrl()
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string secretKey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng";
            string returnUrl = "https://localhost:44394/Home/ConfirmPaymentClient";
            string notifyUrl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; // Lưu ý: notifyUrl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = "10000";
            string orderId = DateTime.Now.Ticks.ToString(); // Mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            // Trước khi ký HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderId + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyUrl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            // Ký chữ ký SHA256
            string signature = crypto.signSHA256(rawHash, secretKey);

            // Xây dựng nội dung JSON request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyUrl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            string paymentUrl = jmessage.GetValue("payUrl").ToString();

            // Trả về kết quả OK cùng với URL
            return Ok(new { PaymentUrl = paymentUrl });
        }
       
    }
}
