using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemBAL.OrderRepositry;
using OrderManagmentSytemDAL.ViewModels;

namespace OrderManagmentSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepositry orderRepositry;
        private readonly ICustomerRepositry customerRepositry;

        public OrderController(IOrderRepositry orderRepositry,ICustomerRepositry customerRepositry)
        {
            this.orderRepositry = orderRepositry;
            this.customerRepositry=customerRepositry;
        }
        [HttpGet]
        [Route("/Orders")]
        public IActionResult OrderList()
        {
            Response response=orderRepositry.GetOrders();
            if(response!=null && response.IsSuccess)
            {
                return View(response);
            }
            return View("Index","Home");
        }

        
        [HttpGet]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder()
        {
            Response customersResponse = customerRepositry.GetCustomers();
            OrderManagmentSytemDAL.ViewModels.CreateOrder createOrder = new CreateOrder();
            createOrder.customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
            createOrder.order = new Order();
            createOrder.order.customer_id = createOrder.customerDetails.FirstOrDefault().CustomerId;
            return View("OrderForm",createOrder);
        }
        
        [HttpPost]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder(Order order)
        {
            if(!ModelState.IsValid)
            {
                Response customersResponse = customerRepositry.GetCustomers();
                CreateOrder createOrder = new CreateOrder
                {
                    customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>,
                    order = order
                };
                return View("OrderForm", createOrder);
            }
            Response response;
            if (order.orderId == 0)
            {
                response = orderRepositry.CreateOrder(order);
            }
            else
            {
                response = orderRepositry.UpdateOrder(order);
            }

            if (response.IsSuccess)
            {
                return RedirectToAction("OrderList");
            }
            else
            {
                Response customersResponse = customerRepositry.GetCustomers();
                CreateOrder createOrder = new CreateOrder
                {
                    customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>,
                    order = order
                };
                return View("OrderForm", createOrder);
            }
        }
        [HttpGet]
        [Route("/Orders/edit")]
        public IActionResult EditOrder(int orderId)
        {
            Response customersResponse = customerRepositry.GetCustomers();
            OrderManagmentSytemDAL.ViewModels.CreateOrder createOrder = new CreateOrder();
            createOrder.customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
            Response response=orderRepositry.GetOrder(orderId);
            if (response.IsSuccess)
            {
                createOrder.order = response.Result as Order;
                return View("OrderForm", createOrder);
            }
            else
            {
                return View("OrderList");
            }
        }
        [HttpGet]
        [Route("/Orders/deleteconfirmation")]
        public IActionResult DeleteConfirmationOrder(int orderId)
        {
            if (orderRepositry.GetOrder(orderId).IsSuccess)
            {
                return PartialView("ConfirmationBox", orderId);
            }
            return Json(new { message = "Order Not Exits" });
        }

        [HttpGet]
        [Route("/Orders/delete")]
        public IActionResult DeleteOrder(int orderId)
        {
            if (orderRepositry.GetOrder(orderId).IsSuccess)
            {
                Response response = orderRepositry.DeleteOrder(orderId);
                if(response.IsSuccess)
                {
                    return RedirectToAction("OrderList");
                }
                else
                {
                    return RedirectToAction("Privacy", "Home");
                }
            }
            else
            {
                return RedirectToAction("Privacy", "Home");
            }
        }
    }
}
