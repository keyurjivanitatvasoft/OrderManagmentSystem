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
            createOrder.CustomerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
            createOrder.Order = new Order();
            createOrder.Order.CustomerId = createOrder.CustomerDetails.FirstOrDefault().CustomerId;
            return View("OrderForm", createOrder);
        }

        [HttpPost]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder(Order order )
        {
            if (!ModelState.IsValid)
            {
                Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
                IEnumerable<CustomerDetails> customerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
                CreateOrder createOrder = new CreateOrder
                {
                    CustomerDetails = customerDetails,
                    Order = order
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
                    CustomerDetails = customersResponse.Result as IEnumerable<CustomerDetails>,
                    Order = order
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
                OrderId = orderId
            };
            Response orderResponse = orderRepositry.SearchOrderSP(searchOrder);
            if (orderResponse.IsSuccess && orderResponse.Result is IEnumerable<OrderWithCustomer> orders && orders.Count() == 1)
            {
                CreateOrder viewModel = new CreateOrder
                {
                    CustomerDetails = customerDetails,
                    Order = new Order
                    {
                        OrderId = orders.First().OrderId,
                        ProductName = orders.First().ProductName,
                        Quantity = orders.First().Quantity,
                        Amount = orders.First().Amount,
                        CustomerId= orders.First().CustomerId,
                    },
                };
                return View("OrderForm", viewModel);
            }
            return RedirectToAction("OrderList");
        }
        [HttpPost]
        [Route("/Orders/deleteconfirmation")]
        public IActionResult DeleteConfirmationOrder(List<int> orderIds)
        {
            Response order = orderRepositry.OrdersExits(orderIds);

            if (order.IsSuccess )
            {
                return PartialView("ConfirmationBox", orderIds);
            }
            return Json(new { message = "Order Not Exits" });
        }

        [HttpPost]
        [Route("/Orders/delete")]
        public IActionResult DeleteOrder(List<int> orderIds)
        {
            Response orders = orderRepositry.OrdersExits(orderIds);

            if (orders.IsSuccess)
            {
                Response DeleteOrders = orderRepositry.DeleteOrders(orderIds);
                if (DeleteOrders.IsSuccess)
                {
                    return RedirectToAction("OrderList");
                }
                else
                {
                    return PartialView("ConfirmationBox", orderIds);
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
