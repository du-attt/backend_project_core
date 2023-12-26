using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("WishlishDetails")]
    public class WishlishDetails
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int idWishlish { get; set; }
        [Required]
        public string _idProduct { get; set; }

        //
        public Wishlists Wishlists { get; set; }
        public Products Products { get; set; }
    }
}
