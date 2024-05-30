using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class CustomersOrderDetails
    {
        public int CustomerId {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long TotalAmount { get; set; }
    }
}
