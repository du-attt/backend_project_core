using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Users")]
    public class Users
    {
        [Key]

        public int id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }
}
