using Microsoft.AspNetCore.Http;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.CustomerRepositry
{
    public interface ICustomerRepositry
    {
        #region UploadFiles
        string UploadFile(IFormFile file);
        #endregion
        #region simple CRUD (Task 1)
        Response CustomerExits(int customerId, string emailId, string phoneNumber);
        Response GetCustomers();
        Response GetCustomer(int customerId);
        Response DeleteCustomer(int customerId);
        Response AddCustomer(CustomerDetails customerDetails);
        Response EditCustomer(CustomerDetails customerDetails);
        #endregion

        #region CRUD SP (Task 2)
        Response SearchCustomerSP(CustomerDetails customer);
        Response SaveCustomersSP(CustomerDetails customer, bool Isdelete);
        #endregion

        #region Task 4

        Response CustomersExits(List<int> customerIds);
        Response DeleteCustomers(List<int> customerIds);
        #endregion
    }
}
