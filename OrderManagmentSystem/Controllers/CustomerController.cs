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
            Response customerDetails = customerRepositry.GetCustomers();
            return View(customerDetails);
        }
        [HttpGet]
        [Route("/Customers/edit")]
        public IActionResult EditCustomer(int customerId)
        {
            Response customerDetails = customerRepositry.GetCustomer(customerId);
            if (customerDetails.IsSuccess)
            {
                return View("CustomerForm",customerDetails.Result);
            }
            else
            {
                return RedirectToAction("CustomerList");
            }
        }
        [HttpGet]
        [Route("/Customers/deleteconfirmation")]
        public IActionResult DeleteConfirmationCustomer(int customerId)
        {
            if (customerRepositry.GetCustomer(customerId).IsSuccess)
            {
                return PartialView("ConfirmationBox", customerId);
            }
            return Json(new {message="Customer Not Exits"});
        }

        [HttpGet]
        [Route("/Customers/delete")]
        public IActionResult DeleteCustomer(int customerId)
        {
            if (customerRepositry.GetCustomer(customerId).IsSuccess)
            {
                Response customerDetails = customerRepositry.DeleteCustomer(customerId);
                if (customerDetails.IsSuccess)
                {
                    return RedirectToAction("CustomerList");
                }
                else
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
            Response response;
            if (customerDetails.CustomerId == 0)
            {
                if(customerRepositry.CustomerExits(customerDetails.Email,customerDetails.PhoneNumber).IsSuccess)
                {
                    ModelState.AddModelError("CustomerId", "Customer with this email or phone number already exists.");
                    return View("CustomerForm", customerDetails);
                }
                response = customerRepositry.AddCustomer(customerDetails);
            }
            else
            {
                response = customerRepositry.EditCustomer(customerDetails);
            }

            if (response.IsSuccess)
            {
                return RedirectToAction("CustomerList");
            }
            else
            {
                return View("CustomerForm", customerDetails);
            }

        }



    }
}
