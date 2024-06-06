using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemDAL.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace OrderManagmentSystem.Controllers
{
    public class CustomerController : Controller

    {
        private readonly ICustomerRepositry customerRepositry;
        private readonly INotyfService notyf;

        public CustomerController(ICustomerRepositry customerRepositry,INotyfService notyfService)
        {
            this.customerRepositry = customerRepositry;
            this.notyf = notyfService;
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
            notyf.Error("Customer not Found");
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
            notyf.Error("Customer not Found");
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
                    notyf.Success("Customer Deleted Successfully.");
                    return RedirectToAction("CustomerList");
                }
               
            }
            notyf.Error("Customer not Found");
            return RedirectToAction("CustomerList");
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
            if(customerDetails.CustomerId!=0 && !customerRepositry.CustomersExits(new List<int>(customerDetails.CustomerId)).IsSuccess)
            {
                notyf.Error("Customer not Found");
                return RedirectToAction("CustomerList");
            }
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
                if (customerDetails.CustomerId == 0)
                {
                    notyf.Success("Customer Add Successfully.");
                }
                else
                {
                    notyf.Success("Edit Customer Successfully.");
                }
                return RedirectToAction("CustomerList");
            }
            if (customerDetails.CustomerId == 0)
            {
                notyf.Error("Customer not Added");
            }
            else
            {
                notyf.Error("Customer not Edited");
            }
            return View("CustomerForm", customerDetails);
        }



    }
}
