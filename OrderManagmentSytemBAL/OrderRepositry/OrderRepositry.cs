using Microsoft.VisualBasic.FileIO;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.ViewModels;
using System.Net;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace OrderManagmentSytemBAL.OrderRepositry
{
    public class OrderRepositry : IOrderRepositry
    {
        private readonly OrderManagmentSystemDapperContext context;
        public OrderRepositry(IOptions<ConnectionStrings> connectionStrings)
        {
            context = new OrderManagmentSystemDapperContext(connectionStrings.Value.OrderManagmentSystem);
        }
        #region simple CRUD (Task 1)
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
                int rows = context.Execute("UPDATE [Order] SET Isdelete=1  WHERE orderId = @orderId", new { OrderId = orderId });
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
                IEnumerable<OrderWithCustomer> orders = context.Query<OrderWithCustomer>("select * From [Order] join Customer on [Order].customer_id=Customer.customer_id where [Order].Isdelete=0", null, false);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = orders;
            }
            catch (Exception ex)
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

                Order order = context.QuerySingle<Order>("SELECT  * FROM [Order] WHERE orderId = @OrderId  AND Isdelete=0", new { OrderId = orderId });
                if (order != null)
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


        #endregion

        #region CRUD SP (Task 2)
        public Response SearchOrderSP(Order order)
        {
            Response response = new Response();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductName", order.ProductName);
                parameters.Add("@Amount", order.Amount);
                parameters.Add("@Quantity", order.Quantity);
                parameters.Add("@OrderId", order.OrderId);
                parameters.Add("@CustomerId", order.CustomerId);

                IEnumerable<OrderWithCustomer> customersData = context.Query<OrderWithCustomer>("SearchOrder", parameters, true);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = customersData;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
        public Response SaveOrdersSP(Order order, bool Isdelete)
        {
            Response response = new Response();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductName", order.ProductName);
                parameters.Add("@Amount", order.Amount);
                parameters.Add("@Quantity", order.Quantity);
                parameters.Add("@OrderId", order.OrderId);
                parameters.Add("@CustomerId", order.CustomerId);
                parameters.Add("@IsDelete", Isdelete);

                int rowaffected = context.Execute("SaveOrder", parameters, true);
                if (rowaffected > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        #endregion

        #region customerOrderDetailsc (Task 3)
        public Response CustomerOrderDetails()
        {
            Response response = new Response();
            try
            {
                IEnumerable<CustomersOrderDetails> customersData = context.Query<CustomersOrderDetails>("CustomerOrderDetails", null, true);
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = customersData;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;

        }
        #endregion

        #region Task 4

        public Response OrdersExits(List<int> orderIds)
        {
            Response response = new Response();
            try
            {
                DataTable orders = new DataTable();
                orders.Columns.Add(new DataColumn("Value", typeof(int)));
                foreach (var id in orderIds)
                {
                    orders.Rows.Add(id);
                }
                var parameters = new DynamicParameters();
                parameters.Add("@OrderIds", orders.AsTableValuedParameter("IntListType"));
                int ordersCount = context.QuerySingle<int>("OrdersExits", parameters, true);
                if(ordersCount == orderIds.Count())
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response DeleteOrders(List<int> orderIds)
        {
            Response response = new Response();
            try
            {
                DataTable orders = new DataTable();
                orders.Columns.Add(new DataColumn("Value", typeof(int)));
                foreach (var id in orderIds)
                {
                    orders.Rows.Add(id);
                }
                var parameters = new DynamicParameters();
                parameters.Add("@OrderIds", orders.AsTableValuedParameter("IntListType"));
                int rowsaffected = context.Execute("DeleteOrders", parameters, true);
                if (rowsaffected>0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
        #endregion

    }
}
