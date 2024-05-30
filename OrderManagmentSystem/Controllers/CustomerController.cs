using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemDAL.ViewModels;

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
        [HttpGet]
        [Route("/Customers/deleteconfirmation")]
        public IActionResult DeleteConfirmationCustomer(int customerId)
        {
            CustomerDetails customer = new CustomerDetails
            {
                CustomerId = customerId,
            };
            Response searchCustomer = customerRepositry.SearchCustomerSP(customer);
            if (searchCustomer.IsSuccess && searchCustomer.Result is IEnumerable<CustomerDetails> customers && customers.Count()==1)
            {
                return PartialView("ConfirmationBox", customerId);
            }
            return Json(new {message="Customer Not Exits"});
        }

        [HttpGet]
        [Route("/Customers/delete")]
        public IActionResult DeleteCustomer(int customerId)
        {
            CustomerDetails customer = new CustomerDetails
            {
                CustomerId = customerId,
            };
            Response searchCustomer = customerRepositry.SearchCustomerSP(customer);
            if (searchCustomer.IsSuccess && searchCustomer.Result is IEnumerable<CustomerDetails> customers && customers.Count() == 1)
            {
                Response customerDetails = customerRepositry.SaveCustomersSP(customers.First(),true);
                if (customerDetails.IsSuccess)
                {
                    return RedirectToAction("CustomerList");
                }
            }
            return RedirectToAction("Privacy","Home");
        }
        [HttpGet]
        [Route("/Customers/Addcustomer")]
        public IActionResult CreateCustomer()
        {
            CustomerDetails customer = new CustomerDetails();
                return View("CustomerForm",customer);
           
        }
        [HttpPost]
        [Route("/Customers/Addcustomer")]
        public IActionResult CreateCustomer(CustomerDetails customerDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customerDetails);
            }
            Response customerCheck= customerRepositry.CustomerExits(customerDetails.CustomerId,customerDetails.Email, customerDetails.PhoneNumber);
            if (customerCheck.IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer with this email or phone number already exists.");
                return View("CustomerForm", customerDetails);
            }
            Response response=customerRepositry.SaveCustomersSP(customerDetails,false);
            if (response.IsSuccess)
            {
                return RedirectToAction("CustomerList");
            }
            return View("CustomerForm", customerDetails);
        }



    }
}
