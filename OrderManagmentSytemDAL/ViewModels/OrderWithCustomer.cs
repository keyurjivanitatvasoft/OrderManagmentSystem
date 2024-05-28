using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class OrderWithCustomer
    {
        public int orderId { get; set; }

        public string productName { get; set; }
        public double amount { get; set; }
        public int quantity { get; set; }

        public int customer_id {  get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
