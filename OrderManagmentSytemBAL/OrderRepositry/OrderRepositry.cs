using Microsoft.VisualBasic.FileIO;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.ViewModels;
using System.Net;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace OrderManagmentSytemBAL.OrderRepositry
{
    public class OrderRepositry:IOrderRepositry
    {
        private readonly OrderManagmentSystemDapperContext context;
        public OrderRepositry(IOptions<ConnectionStrings> connectionStrings)
        {
            
            
            context = new OrderManagmentSystemDapperContext(connectionStrings.Value.OrderManagmentSystem);
        }

        public Response CreateOrder(Order order)
        {
            Response response = new Response();
            try
            {
                int rows = context.Execute("insert into [Order](customer_id,productName,amount,quantity) values (@customer_id,@productName,@amount,@quantity)", order);
                if (rows > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Order not Added" };
                }
            }
                catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response DeleteOrder(int orderId)
        {
            Response response = new Response();
            try
            {
                int rows = context.Execute("UPDATE [Order] SET Isdelete=1  WHERE orderId = @orderId", new { OrderId=orderId });
                if (rows > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Order not Delete" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response GetOrders()
        {
            Response response = new Response();
            try
            {
                IEnumerable<OrderWithCustomer> orders = context.Query<OrderWithCustomer>("select * From [Order] join Customer on [Order].customer_id=Customer.customer_id where [Order].Isdelete=0");

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = orders;
            }
            catch(Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response GetOrder(int orderId)
        {
            Response response = new Response();
            try
            {

                Order order=context.QuerySingle<Order>("SELECT  * FROM [Order] WHERE orderId = @OrderId  AND Isdelete=0", new { OrderId = orderId });
                if (order!=null)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Result = order;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Order not found" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response UpdateOrder(Order order)
        {
            Response response = new Response();
            try
            {
                int rows = context.Execute("UPDATE [Order] SET customer_id = @customer_id, productName = @productName, amount = @amount, quantity = @quantity WHERE orderId = @orderId", order);
                if (rows > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Order not Updated" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

      
    }
}
