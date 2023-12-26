using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Carts")]
    public class Carts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [ForeignKey("idUser")]
        public int idUser { get; set; }

        //Lien ket
        public Users Users { get; set; }
        //Một cart có nhiều cartdetail
        public ICollection<CartDetails> CartDetails { get; set; }

        public Carts() 
        {
            CartDetails = new List<CartDetails>();
        }
    }
}
