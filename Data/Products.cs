using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Products")]
    public class Products
    {
        [Key]
        [MaxLength(256)]
        public string _id { get; set; }

        public int qty { get; set; }

        [MaxLength(255)]
        public string name { get; set; }
        [MaxLength(555)]
        public string? description { get; set; }
        [MaxLength(255)]
        public string brand { get; set; }
        [MaxLength(255)]
        public string category { get; set; }
        [MaxLength(255)]
        public string gender { get; set; }

        [MaxLength(10)]
        public string weight { get; set; }

        public int quantity { get; set; }
        [MaxLength(255)]
        public string image { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        public decimal rating { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal price { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal newPrice { get; set; }

        public bool trending { get; set; }


        // Liên kết đến bảng khác(Khóa ngoại của bảng kia)
        public ICollection<CartDetails> CartDetails { get; set; }
        public ICollection<WishlishDetails> WishlishDetails { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }


        // xác định một product có thể thuộc nhiều cartdetail

        public Products()
        {
            CartDetails = new List<CartDetails>();
        }
    }
}
