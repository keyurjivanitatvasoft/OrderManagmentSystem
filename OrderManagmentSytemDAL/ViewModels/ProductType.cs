using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }
        [Required(ErrorMessage = "Product type name is required")]
        [RegularExpression(@"^(?=.*[^\s])[a-zA-Z\s]*$", ErrorMessage = "Product type name must contain only letters.")]
        [StringLength(50, ErrorMessage = "Product type name must not exceed 50 characters.")]
        public string ProductTypeName { get; set; }
    }
}
