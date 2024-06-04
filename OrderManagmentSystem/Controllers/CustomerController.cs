using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemDAL.ViewModels;
using Microsoft.AspNetCore.Http.Features;
namespace OrderManagmentSystem.Controllers
{
    public class CustomerController : Controller

    {
        private readonly ICustomerRepositry customerRepositry;

        public CustomerController(ICustomerRepositry customerRepositry)
        {
            this.customerRepositry = customerRepositry;
        }

        [HttpGet]
        [Route("/Customers")]
        public IActionResult CustomerList()
        {
            Response customerDetails = customerRepositry.SearchCustomerSP(new CustomerDetails());
            return View(customerDetails);
        }
        [HttpGet]
        [Route("/Customers/edit")]
        public IActionResult EditCustomer(int customerId)
        {
            CustomerDetails customer = new CustomerDetails
            {
                CustomerId = customerId,
            };
            Response customerDetailsResponse = customerRepositry.SearchCustomerSP(customer);
            if (customerDetailsResponse.IsSuccess && customerDetailsResponse.Result is IEnumerable<CustomerDetails> customers && customers.Count() == 1)
            {
                return View("CustomerForm", customers.First());
            }
            return RedirectToAction("CustomerList");
        }
        [HttpPost]
        [Route("/Customers/deleteconfirmation")]
        public IActionResult DeleteConfirmationCustomer(List<int> customerIds)
        {
            Response searchCustomer = customerRepositry.CustomersExits(customerIds);
            if (searchCustomer.IsSuccess)
            {
                return PartialView("ConfirmationBox", customerIds);
            }
            return Json(new { message = "Customer Not Exits" });
        }

        [HttpPost]
        [Route("/Customers/delete")]
        public IActionResult DeleteCustomer(List<int> customerIds)
        {
            Response searchCustomer = customerRepositry.CustomersExits(customerIds);
            if (searchCustomer.IsSuccess)
            {
                Response customerDetails = customerRepositry.DeleteCustomers(customerIds);
                if (customerDetails.IsSuccess)
                {
                    return RedirectToAction("CustomerList");
                }
                else
                {
                    return PartialView("ConfirmationBox", customerIds);
                }
            }
            return RedirectToAction("Privacy", "Home");
        }
        [HttpGet]
        [Route("/Customers/Addcustomer")]
        public IActionResult CreateCustomer()
        {
            CustomerDetails customer = new CustomerDetails();
            return View("CustomerForm", customer);

        }
        [HttpPost]
        [Route("/Customers/Addcustomer")]
        public IActionResult CreateCustomer(CustomerDetails customerDetails, IFormFile? ProfilePhoto)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customerDetails);
            }
            Response customerCheck = customerRepositry.CustomerExits(customerDetails.CustomerId, customerDetails.EmailId, customerDetails.PhoneNumber);
            if (customerCheck.IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer with this email or phone number already exists.");
                return View("CustomerForm", customerDetails);
            }
            if (!(ProfilePhoto == null || !(ProfilePhoto.Length > 0)))
            {
                string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };
                string[] _permittedMimeTypes = { "image/jpeg", "image/png" };
                var fileExtension = System.IO.Path.GetExtension(ProfilePhoto.FileName).ToLowerInvariant();
                var fileContentType = ProfilePhoto.ContentType.ToLowerInvariant();

                if (!_permittedExtensions.Contains(fileExtension) || !_permittedMimeTypes.Contains(fileContentType))
                {
                    ModelState.AddModelError("Photo", "only png and jpg  format file upload.");
                    return View("CustomerForm", customerDetails);
                }
                string fileName = customerRepositry.UploadFile(ProfilePhoto);
                if (fileName == null && customerDetails.CustomerId == 0)
                {
                    ModelState.AddModelError("Photo", "Error in Uploading File.");
                    return View("CustomerForm", customerDetails);
                }
                customerDetails.Photo = fileName;
            }
            Response response = customerRepositry.SaveCustomersSP(customerDetails, false);
            if (response.IsSuccess)
            {
                return RedirectToAction("CustomerList");
            }
            return View("CustomerForm", customerDetails);
        }



    }
}
