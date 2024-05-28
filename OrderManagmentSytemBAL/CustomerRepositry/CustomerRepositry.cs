using Microsoft.Extensions.Options;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.CustomerRepositry
{
    public class CustomerRepositry : ICustomerRepositry
    {
        private readonly OrderManagmentSystemContext context;
        public CustomerRepositry(IOptions<ConnectionStrings> connectionStrings)
        {
            context = new OrderManagmentSystemContext(connectionStrings.Value.OrderManagmentSystem);
        }
        public Response GetCustomers()
        {
            Response response = new Response();

            try
            {
                DataTable customers = context.ExecuteReader("Select * From Customer  Where Isdelete=0");
                List<CustomerDetails> customersList = new List<CustomerDetails>();

                foreach (DataRow row in customers.Rows)
                {
                    CustomerDetails customer = new CustomerDetails
                    {
                        CustomerId = Convert.ToInt32(row["customer_id"]),
                        FirstName = row["firstName"].ToString(),
                        LastName = row["lastName"].ToString(),
                        Email = row["emailId"].ToString(),
                        PhoneNumber = row["phoneNumber"].ToString(),
                        Address = row["address"].ToString(),
                    };

                    customersList.Add(customer);
                }
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = customersList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response GetCustomer(int customerId)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@customerId", customerId)
        };
                DataTable customer = context.ExecuteReader("Select * From Customer where customer_id=@customerId AND Isdelete=0", parameters);

                if (customer.Rows.Count > 0)
                {
                    DataRow customerRow = customer.Rows[0];
                    CustomerDetails customerDetails = new CustomerDetails()
                    {
                        CustomerId = Convert.ToInt32(customerRow["customer_id"]),
                        FirstName = customerRow["firstName"].ToString(),
                        LastName = customerRow["lastName"].ToString(),
                        Email = customerRow["emailId"].ToString(),
                        PhoneNumber = customerRow["phoneNumber"].ToString(),
                        Address = customerRow["address"].ToString()
                    };

                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Result = customerDetails;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer not found" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response DeleteCustomer(int customerId)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@customerId", customerId)
        };
                int row = context.ExecuteNonQuery("UPDATE Customer SET Isdelete=1  WHERE  customer_id = @customerId", parameters);

                if (row > 0)
                {

                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer not found" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response AddCustomer(CustomerDetails customerDetails)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@firstName", customerDetails.FirstName),
                    new SqlParameter("@lastName", customerDetails.LastName),
                    new SqlParameter("@emailId", customerDetails.Email),
                    new SqlParameter("@phoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@address", customerDetails.Address),
                };
                int row = context.ExecuteNonQuery("insert into Customer (firstName,lastName,emailId,phoneNumber,address) values (@firstName,@lastName,@emailId,@phoneNumber,@address)", parameters);
                if (row > 0)
                {

                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer not Added" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response EditCustomer(CustomerDetails customerDetails)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@firstName", customerDetails.FirstName),
                    new SqlParameter("@lastName", customerDetails.LastName),
                    new SqlParameter("@emailId", customerDetails.Email),
                    new SqlParameter("@phoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@address", customerDetails.Address),
                    new SqlParameter("@CustomerId", customerDetails.CustomerId),
                };
                int row = context.ExecuteNonQuery("update Customer set firstName=@firstName,lastName=@lastName,emailId=@emailId,phoneNumber=@phoneNumber,address=@address where  Customer_Id = @CustomerId", parameters);
                if (row > 0)
                {

                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer not Added" };
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        public Response CustomerExits(string emailId, string phoneNumber)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@emailId", emailId),
            new SqlParameter("@phoneNumber", phoneNumber)
        };
                DataTable customer = context.ExecuteReader("Select * From Customer where emailId=@emailId OR phoneNumber=@phoneNumber", parameters);

                if (customer.Rows.Count > 0)
                {
                    

                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer found" };
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
