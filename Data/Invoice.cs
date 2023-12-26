using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_project_core.Data
{
    [Table("Invoice")]
    public class Invoice
    {
        private static int invoiceCounter = 1;
        [Key]
        public string Id { get; set; } // Id hóa đơn
        public int idUser { get; set; } // Id khách hàng
        public string Company { get; set; } // Công ty bán
        public DateTime IssueDate { get; set; } // Ngày phát hành hóa đơn
        public DateTime DueDate { get; set; }   // Hạn thanh toán
        public decimal TotalAmount { get; set; } // Tổng giá trị của hóa đơn
        public decimal ShippingFee { get; set; } // Phí vận chuyển (nếu có)
        public decimal GrandTotal { get; set; }  // Tổng cộng cần thanh toán
        public string PaymentMethod { get; set; } // Phương thức thanh toán
        public string CustomerName { get; set; }  // Tên khách hàng
        public string CustomerAddress { get; set; } // Địa chỉ khách hàng


        // Liên kết đến bảng User
        public Users Users { get; set; }
        //
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        // Một Invoice có thể có nhiều chi tiết hóa đơn
        public ICollection<CartDetails> CartDetails { get; set; }

        // Constructor để khởi tạo giá trị mặc định
        public Invoice()
        {
            Id = GenerateInvoiceId();
            // Khởi tạo giá trị mặc định cho ngày phát hành hóa đơn (ví dụ: ngày hiện tại)
            IssueDate = DateTime.Now;
            // Hạn thanh toán cách 5 ngày kể từ khi tạo hóa đơn
            DueDate = IssueDate.AddDays(5);
        }

        private string GenerateInvoiceId()
        {
            // Tạo chuỗi ID với định dạng #HD01, #HD02, ...
            string newId = $"HD{invoiceCounter:D2}";

            // Tăng biến đếm để chuẩn bị cho ID tiếp theo
            invoiceCounter++;

            return newId;
        }


    }
}
