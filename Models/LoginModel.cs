using System.ComponentModel.DataAnnotations;

namespace backend_project_core.Models
{
    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
