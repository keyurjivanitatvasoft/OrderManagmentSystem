using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class CreateOrder
    {
        public Order order {  get; set; }
        public IEnumerable<CustomerDetails> customerDetails { get; set; }
    }
}
