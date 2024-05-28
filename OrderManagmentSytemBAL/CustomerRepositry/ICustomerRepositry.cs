using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.CustomerRepositry
{
    public  interface ICustomerRepositry
    {
        Response CustomerExits(int customerId,string emailId, string phoneNumber);
        Response GetCustomers();
        Response GetCustomer(int customerId);
        Response DeleteCustomer(int customerId);
        Response AddCustomer(CustomerDetails customerDetails);
        Response EditCustomer(CustomerDetails customerDetails);
        Response SearchCustomerSP(CustomerDetails customer);
        Response SaveCustomersSP(CustomerDetails customer);
    }
}
