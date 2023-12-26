
using backend_project_core.Data;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly MyDbContext _userDbContext;

        public RegisterController(MyDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }
        [HttpPost]
        [Route("register")]
        public string Register(RegisterModel model)
        {

            // Lấy dữ liệu từ sql lên
            var existingUser = _userDbContext.Users.SingleOrDefault(p => p.email == model.email);
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (existingUser != null)
            {
                return "Email already exists!";
            }

            // Tạo một đối tượng User từ dữ liệu model
            var user = new Users
            {
                // Gán các thuộc tính từ model
                name = model.name,
                password = Users.HashPassword(model.password),//chuyển đổi pass
                email = model.email,

                // Thời gian tạo và cập nhật tài khoản được tạo bằng ngày tạo
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };


            // Lưu vào cơ sở dữ liệu
            _userDbContext.Users.Add(user);
            _userDbContext.SaveChanges();

            // Tạo giỏ hàng ứng với user
            var userID = user.id;
            var cart = new Carts
            {
                idUser = userID,
            };
            _userDbContext.Carts.Add(cart);
            _userDbContext.SaveChanges();
            // Tạo Wishlist ứng với Users
            var Wishlist = new Wishlists
            {
                idUser = userID,
            };
            _userDbContext.Wishlists.Add(Wishlist);
            _userDbContext.SaveChanges();
            return "Data inserted";
        }

    }
}
