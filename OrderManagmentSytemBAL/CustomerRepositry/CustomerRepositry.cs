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
        #region BASIC CRUD (Task 1)
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
                        CustomerFirstName = row["firstName"].ToString(),
                        CustomerLastName = row["lastName"].ToString(),
                        EmailId = row["emailId"].ToString(),
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
                SqlParameter[] parameters = 
                {
                    new SqlParameter("@customerId", customerId)
                };
                DataTable customer = context.ExecuteReader("Select * From Customer where customer_id=@customerId AND Isdelete=0", parameters,false);

                if (customer.Rows.Count > 0)
                {
                    DataRow customerRow = customer.Rows[0];
                    CustomerDetails customerDetails = new CustomerDetails()
                    {
                        CustomerId = Convert.ToInt32(customerRow["customer_id"]),
                        CustomerFirstName = customerRow["firstName"].ToString(),
                        CustomerLastName = customerRow["lastName"].ToString(),
                        EmailId = customerRow["emailId"].ToString(),
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
                SqlParameter[] parameters = 
                {
                    new SqlParameter("@customerId", customerId)
                };
                int row = context.ExecuteNonQuery("UPDATE Customer SET Isdelete=1  WHERE  customer_id = @customerId", parameters,false);

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
                    new SqlParameter("@firstName", customerDetails.CustomerFirstName),
                    new SqlParameter("@lastName", customerDetails.CustomerLastName),
                    new SqlParameter("@emailId", customerDetails.EmailId),
                    new SqlParameter("@phoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@address", customerDetails.Address),
                };
                int row = context.ExecuteNonQuery("insert into Customer (firstName,lastName,emailId,phoneNumber,address) values (@firstName,@lastName,@emailId,@phoneNumber,@address)", parameters,false);
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
                    new SqlParameter("@firstName", customerDetails.CustomerFirstName),
                    new SqlParameter("@lastName", customerDetails.CustomerLastName),
                    new SqlParameter("@emailId", customerDetails.EmailId),
                    new SqlParameter("@phoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@address", customerDetails.Address),
                    new SqlParameter("@CustomerId", customerDetails.CustomerId),
                };
                int row = context.ExecuteNonQuery("update Customer set firstName=@firstName,lastName=@lastName,emailId=@emailId,phoneNumber=@phoneNumber,address=@address where  Customer_Id = @CustomerId", parameters, false);
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

        public Response CustomerExits(int customerId,string emailId, string phoneNumber)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = 
                {
                    new SqlParameter("@emailId", emailId),
                    new SqlParameter("@phoneNumber", phoneNumber),
                    new SqlParameter("@customerId", customerId)
                };
                DataTable customer = context.ExecuteReader("Select * From Customer where customer_id!=@customerId AND (emailId=@emailId OR phoneNumber=@phoneNumber )", parameters, false);
                if (customer.Rows.Count > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>() { "Customer  found" };
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


        #region SP CRUD (Task 2)
        public Response SearchCustomerSP(CustomerDetails searchCustomer) 
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@firstName", searchCustomer.CustomerFirstName),
                    new SqlParameter("@lastName", searchCustomer.CustomerLastName),
                    new SqlParameter("@phoneNumber", searchCustomer.PhoneNumber),
                    new SqlParameter("@emailId", searchCustomer.EmailId),
                    new SqlParameter("@address", searchCustomer.Address),
                    new SqlParameter("@customerId", searchCustomer.CustomerId),
                };

                DataTable customersData = context.ExecuteReader("search_customers", parameters, true);
                List<CustomerDetails> customersList = new List<CustomerDetails>();

                foreach (DataRow row in customersData.Rows)
                {
                    CustomerDetails customer = new CustomerDetails
                    {
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CustomerFirstName = row["CustomerFirstName"].ToString(),
                        CustomerLastName = row["CustomerLastName"].ToString(),
                        EmailId = row["EmailId"].ToString(),
                        PhoneNumber = row["PhoneNumber"].ToString(),
                        Address = row["Address"].ToString(),
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

        public Response SaveCustomersSP(CustomerDetails customer, bool Isdelete)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@firstName", customer.CustomerFirstName),
                    new SqlParameter("@lastName", customer.CustomerLastName),
                    new SqlParameter("@phoneNumber", customer.PhoneNumber),
                    new SqlParameter("@emailId", customer.EmailId),
                    new SqlParameter("@address", customer.Address),
                    new SqlParameter("@customerId", customer.CustomerId),
                    new SqlParameter("@Isdelete",Isdelete ),
                };

                int rowaffected = context.ExecuteNonQuery("SaveCustomers", parameters, true);
                if (rowaffected > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
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
