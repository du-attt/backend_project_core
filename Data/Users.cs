using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace backend_project_core.Data
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Id tự động tăng dần
        public int id { get; set; }

        [Required]
        [MaxLength(256)]
        public string name { get; set; }

        [Required]
        [MaxLength(256)]
        public string email { get; set; }

        [Required]
        [MaxLength(256)]
        [JsonIgnore] // Ẩn mật khẩu đổi thành JSON
        public string password { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định thời gian tạo tài khoản
        public DateTime created_at { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định thời gian cập nhật tài khoản
        public DateTime updated_at { get; set; }


        // Hàm chuyển đổi mật khẩu thành dạng json 
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
