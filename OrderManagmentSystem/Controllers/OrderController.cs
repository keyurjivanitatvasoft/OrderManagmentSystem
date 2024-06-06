using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService notyf;

        public OrderController(IOrderRepositry orderRepositry, ICustomerRepositry customerRepositry, INotyfService notyfService)
        {
            this.orderRepositry = orderRepositry;
            this.customerRepositry = customerRepositry;
            this.notyf = notyfService;
        }
        [HttpGet]
        [Route("/Orders")]
        public IActionResult OrderList()
        {
            Response response = orderRepositry.SearchOrderSP(new Order());
            return View(response);
        }


        [HttpGet]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder()
        {
            Response customersResponse = customerRepositry.SearchCustomerSP(new CustomerDetails());
            CreateOrder createOrder = new CreateOrder();
            createOrder.CustomerDetails = customersResponse.Result as IEnumerable<CustomerDetails>;
            createOrder.Order = new Order();
            createOrder.Order.CustomerId = -1;
            return View("OrderForm", createOrder);
        }

        [HttpPost]
        [Route("/Orders/Addorder")]
        public IActionResult CreateOrder(Order order)
        {

            if (order.OrderId != 0 && !orderRepositry.OrdersExits(new List<int>(order.OrderId)).IsSuccess)
            {
                notyf.Error("Order not Found");
                return RedirectToAction("OrderList");
            }
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
                if (order.OrderId == 0)
                {
                    notyf.Success("Order Add Successfully.");
                }
                else
                {
                    notyf.Success("Edit Order Successfully.");
                }
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
                if (order.OrderId == 0)
                {
                    notyf.Error("Order not Added");
                }
                else
                {
                    notyf.Error("Order not Edited");
                }
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
                        CustomerId = orders.First().CustomerId,
                    },
                };
                return View("OrderForm", viewModel);
            }
            notyf.Error("Order not Found");
            return RedirectToAction("OrderList");
        }
        [HttpPost]
        [Route("/Orders/deleteconfirmation")]
        public IActionResult DeleteConfirmationOrder(List<int> orderIds)
        {
            Response order = orderRepositry.OrdersExits(orderIds);

            if (order.IsSuccess)
            {
                return PartialView("ConfirmationBox", orderIds);
            }
            notyf.Error("Order not Found");
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
                    notyf.Success("Order deleted successfully");
                }
                else
                {
                    notyf.Error("Order not Deleted");
                }
                return RedirectToAction("OrderList");
            }
            notyf.Error("Order not Found");
            return RedirectToAction("OrderList");
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
