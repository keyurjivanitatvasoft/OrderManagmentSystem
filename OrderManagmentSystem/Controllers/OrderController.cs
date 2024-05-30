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

        public OrderController(IOrderRepositry orderRepositry, ICustomerRepositry customerRepositry)
        {
            this.orderRepositry = orderRepositry;
            this.customerRepositry = customerRepositry;
        }
        [HttpGet]
        [Route("/Orders")]
        public IActionResult OrderList()
        {

            Response response = orderRepositry.SearchOrderSP(new Order());
            if (response != null && response.IsSuccess)
            {
                return View(response);
            }
            return View("Index", "Home");
        }


        [HttpGet]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder()
        {
            Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
            OrderManagmentSytemDAL.ViewModels.CreateOrder createOrder = new CreateOrder();
            createOrder.customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
            createOrder.order = new Order();
            createOrder.order.customer_id = createOrder.customerDetails.FirstOrDefault().CustomerId;
            return View("OrderForm", createOrder);
        }

        [HttpPost]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
                IEnumerable<CustomerDetails> customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
                CreateOrder createOrder = new CreateOrder
                {
                    customerDetails = customerDetails,
                    order = order
                };

                return View("OrderForm", createOrder);
            }


            Response response = orderRepositry.SaveOrdersSP(order, false);

            if (response.IsSuccess)
            {
                return RedirectToAction("OrderList");
            }
            else
            {
                Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
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
            Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
            IEnumerable<CustomerDetails> customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;


            Order searchOrder = new Order
            {
                orderId = orderId
            };
            Response orderResponse = orderRepositry.SearchOrderSP(searchOrder);
            if (orderResponse.IsSuccess && orderResponse.Result is IEnumerable<OrderWithCustomer> orders && orders.Count() == 1)
            {
                CreateOrder viewModel = new CreateOrder
                {
                    customerDetails = customerDetails,
                    order = new Order
                    {
                        orderId = orders.First().orderId,
                        productName = orders.First().productName,
                        quantity = orders.First().quantity,
                        amount = orders.First().amount,
                        customer_id = orders.First().customer_id,
                    },
                };
                return View("OrderForm", viewModel);
            }
            return RedirectToAction("OrderList");
        }
        [HttpGet]
        [Route("/Orders/deleteconfirmation")]
        public IActionResult DeleteConfirmationOrder(int orderId)
        {
            Response order = orderRepositry.SearchOrderSP(new Order { orderId = orderId });

            if (order.IsSuccess && order.Result is IEnumerable<OrderWithCustomer> orders && orders.Count() == 1)
            {
                return PartialView("ConfirmationBox", orderId);
            }
            return Json(new { message = "Order Not Exits" });
        }

        [HttpGet]
        [Route("/Orders/delete")]
        public IActionResult DeleteOrder(int orderId)
        {
            Response order = orderRepositry.SearchOrderSP(new Order { orderId = orderId });

            if (order.IsSuccess && order.Result is IEnumerable<OrderWithCustomer> orders && orders.Count() == 1)
            {
                Order deleteOrder = new Order
                {
                    orderId = orders.First().orderId,
                    productName = orders.First().productName,
                    quantity = orders.First().quantity,
                    amount = orders.First().amount,
                    customer_id = orders.First().customer_id,
                };
                Response response = orderRepositry.SaveOrdersSP(deleteOrder, true);
                if (response.IsSuccess)
                {
                    return RedirectToAction("OrderList");
                }
            }
            return RedirectToAction("Privacy", "Home");
        }

        
        [HttpGet]
        [Route("/customerorders")]
        public IActionResult CustomersOrder()
        {
            Response response = orderRepositry.CustomerOrderDetails();
            IEnumerable<CustomersOrderDetails> customersOrderDetails = response.Result as IEnumerable<CustomersOrderDetails>;
            return View(response);
        }
    }
}
