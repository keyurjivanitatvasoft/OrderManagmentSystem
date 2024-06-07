using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemBAL.OrderRepositry;
using OrderManagmentSytemDAL.DTOs;
using OrderManagmentSytemDAL.ViewModels;
using System.Linq;

namespace OrderManagmentSystem.Controllers
{
    [Route("api/Order")]
    [ApiController]
    [CustomAuthorization]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderRepositry orderRepositry;
        private readonly ICustomerRepositry customerRepositry;
        public OrderApiController(IOrderRepositry orderRepositry,ICustomerRepositry customerRepositry)
        {
            this.orderRepositry = orderRepositry;
            this.customerRepositry = customerRepositry;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            Response customerDetails = orderRepositry.SearchOrderSP(new Order());
            return Ok(customerDetails.Result);
        }
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            Response searchCustomer = orderRepositry.GetOrderById(id);
            if (searchCustomer.IsSuccess)
            {
                return Ok(searchCustomer.Result);
            }
            return BadRequest("Order not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            Response searchCustomer = orderRepositry.GetOrderById(id);
            if (searchCustomer.IsSuccess)
            {
                List<int> Customers = new List<int> { id };
                Response customerDetails = orderRepositry.DeleteOrders(new List<int> { id });
                if (customerDetails.IsSuccess)
                {
                    return Ok("Order deleted successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to delete order.");
                }

            }
            return BadRequest("Order Not Found.");
        }

        [HttpPost]
        public IActionResult AddOrder([FromForm] AddOrderDTO order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!customerRepositry.GetCustomerById(order.CustomerId).IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer not found.");
                return BadRequest(ModelState);
            }
            Response response = orderRepositry.AddOrder(order);
            if(response.IsSuccess)
            {
                return Ok("Order added successfully.");
            }
            else
            {
                return StatusCode(500, "Error occurred while saving order details.");
            }

        }


        [HttpPut("{id}")]
        public IActionResult EditOrder(int id, [FromForm] AddOrderDTO order)
        {
            Response searchCustomer = orderRepositry.GetOrderById(id);
            if (!searchCustomer.IsSuccess)
            {
                return BadRequest("Order not found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!customerRepositry.GetCustomerById(order.CustomerId).IsSuccess)
            {
                ModelState.AddModelError("CustomerId", "Customer not found.");
                return BadRequest(ModelState);
            }
            Response response = orderRepositry.EditOrder(id,order);
            if (response.IsSuccess)
            {
                return Ok("Order Edited successfully.");
            }
            else
            {
                return StatusCode(500, "Error occurred while saving order details.");
            }

        }


    }
}
