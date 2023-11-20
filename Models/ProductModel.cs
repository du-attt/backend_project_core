

namespace backend_project_core.Models
{
    public class ProductModel
    {
        public string _id { get; set; }
        public int qty { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string gender { get; set; }
        public string weight { get; set; }
        public int quantity { get; set; }
        public string image { get; set; }
        public decimal rating { get; set; }
        public decimal price { get; set; }
        public decimal newPrice { get; set; }
        public bool trending { get; set; }
    }
}
