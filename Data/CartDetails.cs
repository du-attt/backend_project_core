using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("CartDetails")]
    public class CartDetails
    {
        [Key]
        [Required]
        public int id { get; set; }
        [ForeignKey("idCart")]

        public int idCart {  get; set; }
        [ForeignKey("_idProduct")]

        public string _idProduct { get; set; }
        public int qty { get; set; }
        public string? color { get; set; }

        //
        public Carts Carts { get; set; }
        public Products Products { get; set; }
    }
}
