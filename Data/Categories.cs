using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        [MaxLength(36)]
        public string _id { get; set; }
        [Required]
        [MaxLength(255)]
        public string categoryName { get; set; }
        [MaxLength(555)]
        public string? description { get; set; }
        [MaxLength(255)]
        public string? categoryImg { get; set; }
    }
}
