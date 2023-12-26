using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Wishlists")]
    public class Wishlists
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("idUser")]
        public int idUser { get; set; }

        public ICollection<WishlishDetails> WishlishDetails { get; set; }

        public Users Users { get; set; }
    }
}
