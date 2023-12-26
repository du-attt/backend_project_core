using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend_project_core.Data
{
    [Table("InvoiceDetail")]
    public class InvoiceDetail
    {
        [Key]
        [Required]
        public int id { get; set; }
        [ForeignKey("idInvoice")]

        public string idInvoice { get; set; }
        [ForeignKey("_idProduct")]

        public string _idProduct { get; set; }
        public int qty { get; set; }

        //
        public Invoice Invoice { get; set; }
        public Products Products { get; set; }
    }
}
