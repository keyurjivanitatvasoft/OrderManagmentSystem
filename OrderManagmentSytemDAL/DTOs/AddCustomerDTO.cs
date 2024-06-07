using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.DTOs
{
    public class AddCustomerDTO
    {

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^\s*[a-zA-Z]+\s*$", ErrorMessage = "First name must contain only letters.")]
        [StringLength(50, ErrorMessage = "First name must not exceed 50 characters.")]
        public string CustomerFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^\s*[a-zA-Z]+\s*$", ErrorMessage = "Last name must contain only letters.")]
        [StringLength(50, ErrorMessage = "Last name must not exceed 50 characters.")]
        public string CustomerLastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\s*\d{10}\s*$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNumber { get; set; }

        public IFormFile? ProfilePic { get; set; }
        public string? Address { get; set; }
    }
}
