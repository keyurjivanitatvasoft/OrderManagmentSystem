using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemDAL.DTOs;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Linq;

namespace OrderManagmentSystem.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [CustomAuthorization]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerRepositry customerRepositry;
        public CustomerApiController(ICustomerRepositry customerRepositry)
        {
            this.customerRepositry = customerRepositry;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            Response customerDetails = customerRepositry.SearchCustomerSP(new CustomerDetails());
            return Ok(customerDetails.Result);
        }
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            Response searchCustomer = customerRepositry.GetCustomerById(id);
            if (searchCustomer.IsSuccess)
            {
                return Ok(searchCustomer.Result);
            }
            return BadRequest("Cusatomer not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            Response searchCustomer = customerRepositry.GetCustomerById(id);
            if (searchCustomer.IsSuccess)
            {
                List<int> Customers = new List<int> { id };
                Response customerDetails = customerRepositry.DeleteCustomers(new List<int> { id });
                if (customerDetails.IsSuccess)
                {
                    return Ok("Customer deleted successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to delete customer.");
                }

            }
            return BadRequest();
        }
        

        [HttpPost]
        public IActionResult AddCustomer([FromForm] AddCustomerDTO customerDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string fileName=null;
            if (!(customerDetails.ProfilePic== null || !(customerDetails.ProfilePic.Length > 0)))
            {
                string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };
                string[] _permittedMimeTypes = { "image/jpeg", "image/png" };
                var fileExtension = System.IO.Path.GetExtension(customerDetails.ProfilePic.FileName).ToLowerInvariant();
                var fileContentType = customerDetails.ProfilePic.ContentType.ToLowerInvariant();

                if (!_permittedExtensions.Contains(fileExtension) || !_permittedMimeTypes.Contains(fileContentType))
                {
                    ModelState.AddModelError("Photo", "only png and jpg  format file upload.");
                    return BadRequest(ModelState);
                }
                fileName = customerRepositry.UploadFile(customerDetails.ProfilePic);
                if (fileName == null)
                {
                    ModelState.AddModelError("Photo", "Error in Uploading File.");
                    return BadRequest(ModelState);
                }
            }
            Response customerCheck = customerRepositry.CustomerExits(0, customerDetails.EmailId, customerDetails.PhoneNumber);
            if (customerCheck.IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer with this email or phone number already exists.");
                return BadRequest(ModelState);
            }

            Response response = customerRepositry.AddCustomer(customerDetails,fileName);
            if (response.IsSuccess)
            {
                return Ok("Customer added successfully.");
            }
            else
            {
                return StatusCode(500, "Error occurred while saving customer details.");
            }
        }
        [HttpPut("{id}")]
        public IActionResult EditCustomer(int id, [FromForm] AddCustomerDTO customerDetails)
        {
            Response searchCustomer = customerRepositry.GetCustomerById(id);
            if (!searchCustomer.IsSuccess)
            {
                return BadRequest("Customer not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string fileName = null;
            if (!(customerDetails.ProfilePic == null || !(customerDetails.ProfilePic.Length > 0)))
            {
                string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };
                string[] _permittedMimeTypes = { "image/jpeg", "image/png" };
                var fileExtension = System.IO.Path.GetExtension(customerDetails.ProfilePic.FileName).ToLowerInvariant();
                var fileContentType = customerDetails.ProfilePic.ContentType.ToLowerInvariant();

                if (!_permittedExtensions.Contains(fileExtension) || !_permittedMimeTypes.Contains(fileContentType))
                {
                    ModelState.AddModelError("Photo", "only png and jpg  format file upload.");
                    return BadRequest(ModelState);
                }
                fileName = customerRepositry.UploadFile(customerDetails.ProfilePic);
                if (fileName == null)
                {
                    ModelState.AddModelError("Photo", "Error in Uploading File.");
                    return BadRequest(ModelState);
                }
            }
            Response customerCheck = customerRepositry.CustomerExits(id, customerDetails.EmailId, customerDetails.PhoneNumber);
            if (customerCheck.IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer with this email or phone number already exists.");
                return BadRequest(ModelState);
            }

            Response response = customerRepositry.EditCustomer(id,customerDetails, fileName);
            if (response.IsSuccess)
            {
                return Ok("Customer Edited successfully.");
            }
            else
            {
                return StatusCode(500, "Error occurred while saving customer details.");
            }
        }
    }
}
