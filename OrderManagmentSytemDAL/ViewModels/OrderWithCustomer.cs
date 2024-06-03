using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class OrderWithCustomer
    {
        public int OrderId { get; set; }
        
        public string ProductName { get; set; }

        public double Amount { get; set; }
        public int Quantity { get; set; }

        public int CustomerId{  get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
    }
}
