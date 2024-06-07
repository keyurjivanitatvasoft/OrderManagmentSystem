using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OrderManagmentSytemBAL.Services;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.DTOs;
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

        #region UploadFiles 
        public string UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CustomerProfilePhotos");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                string fileName = FileUploadServices.GetFilenameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return "/CustomerProfilePhotos/" + fileName;
            }
            return null;
        }
        #endregion

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
                DataTable customer = context.ExecuteReader("Select * From Customer where customer_id=@customerId AND Isdelete=0", parameters, false);

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
                int row = context.ExecuteNonQuery("UPDATE Customer SET Isdelete=1  WHERE  customer_id = @customerId", parameters, false);

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
                int row = context.ExecuteNonQuery("insert into Customer (firstName,lastName,emailId,phoneNumber,address) values (@firstName,@lastName,@emailId,@phoneNumber,@address)", parameters, false);
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

        public Response CustomerExits(int customerId, string emailId, string phoneNumber)
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
                   new SqlParameter("@FirstName", searchCustomer.CustomerFirstName),
                    new SqlParameter("@LastName", searchCustomer.CustomerLastName),
                    new SqlParameter("@PhoneNumber", searchCustomer.PhoneNumber),
                    new SqlParameter("@EmailId", searchCustomer.EmailId),
                    new SqlParameter("@Address", searchCustomer.Address),
                    new SqlParameter("@CustomerId", searchCustomer.CustomerId),
                };

                DataTable customersData = context.ExecuteReader("SearchCustomers", parameters, true);
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
                        Photo = row["Photo"].ToString(),
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
                   new SqlParameter("@FirstName", customer.CustomerFirstName),
                    new SqlParameter("@LastName", customer.CustomerLastName),
                    new SqlParameter("@PhoneNumber", customer.PhoneNumber),
                    new SqlParameter("@EmailId", customer.EmailId),
                    new SqlParameter("@Address", customer.Address),
                    new SqlParameter("@CustomerId", customer.CustomerId),
                    new SqlParameter("@IsDelete",Isdelete ),
                    new SqlParameter("@Photo",customer.Photo ),

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

        #region Task 4

        public Response CustomersExits(List<int> customerIds)
        {
            Response response = new Response();
            try
            {
                DataTable customers = new DataTable();
                customers.Columns.Add(new DataColumn("Value", typeof(int)));
                foreach (int id in customerIds)
                {
                    customers.Rows.Add(id);
                }
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter
                    {
                            ParameterName = "@CustomerIds",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "IntListType",
                            Value = customers
                    }

                };

                int customersCount = context.ExecuteScalar<int>("CustomersExits", parameters, true);
                if (customersCount == customerIds.Count())
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
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return response;
        }
        public Response DeleteCustomers(List<int> customerIds)
        {
            Response response = new Response();
            try
            {
                DataTable customers = new DataTable();
                customers.Columns.Add(new DataColumn("Value", typeof(int)));
                foreach (int id in customerIds)
                {
                    customers.Rows.Add(id);
                }
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter
                    {
                            ParameterName = "@CustomerIds",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "IntListType",
                            Value = customers
                    }

                };

                int rowAffected = context.ExecuteNonQuery("DeleteCustomers", parameters, true);
                if (rowAffected > 0)
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
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }

        #endregion

        #region API (Task 5 )
        public Response GetCustomerById(int customerId)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CustomerId", customerId),
                };

                DataTable customersData = context.ExecuteReader("SearchCustomers", parameters, true);
                if (customersData.Rows.Count == 1)
                {
                    CustomerDetails customer = new CustomerDetails
                    {
                        CustomerId = Convert.ToInt32(customersData.Rows[0]["CustomerId"]),
                        CustomerFirstName = customersData.Rows[0]["CustomerFirstName"].ToString(),
                        CustomerLastName = customersData.Rows[0]["CustomerLastName"].ToString(),
                        EmailId = customersData.Rows[0]["EmailId"].ToString(),
                        PhoneNumber = customersData.Rows[0]["PhoneNumber"].ToString(),
                        Address = customersData.Rows[0]["Address"].ToString(),
                        Photo = customersData.Rows[0]["Photo"].ToString(),
                    };
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Result = customer;

                }
                else
                {

                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
        public Response AddCustomer(AddCustomerDTO customerDetails, string? fileName)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                   new SqlParameter("@FirstName", customerDetails.CustomerFirstName),
                    new SqlParameter("@LastName", customerDetails.CustomerLastName),
                    new SqlParameter("@PhoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@EmailId", customerDetails.EmailId),
                    new SqlParameter("@Address", customerDetails.Address),
                    new SqlParameter("@IsDelete",false ),
                    new SqlParameter("@Photo",fileName ),

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

        public Response EditCustomer(int id, AddCustomerDTO customerDetails, string? fileName)
        {
            Response response = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                   new SqlParameter("@FirstName", customerDetails.CustomerFirstName),
                    new SqlParameter("@LastName", customerDetails.CustomerLastName),
                    new SqlParameter("@PhoneNumber", customerDetails.PhoneNumber),
                    new SqlParameter("@EmailId", customerDetails.EmailId),
                    new SqlParameter("@Address", customerDetails.Address),
                    new SqlParameter("@CustomerId", id),
                    new SqlParameter("@IsDelete",false ),
                    new SqlParameter("@Photo",fileName ),

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
