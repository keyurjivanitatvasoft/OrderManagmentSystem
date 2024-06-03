using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class Order
    {

        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [RegularExpression(@"^\s*[a-zA-Z\s]+\s*$", ErrorMessage = "Product name must contain only letters.")]
        [StringLength(50, ErrorMessage = "Product name must not exceed 50 characters.")]
        public string ProductName { get; set; }

        
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Customer ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be greater than 0")]
        public int CustomerId {  get; set; }
    }
}
