
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
        public string Register(Users user)
        {
            _userDbContext.Users.Add(user);
            _userDbContext.SaveChanges();

            return "Data inserted";
        }
    }
}
