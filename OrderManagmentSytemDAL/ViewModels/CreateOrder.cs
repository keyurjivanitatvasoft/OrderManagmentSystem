using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class CreateOrder
    {
        public Order Order {  get; set; }
        public IEnumerable<CustomerDetails> CustomerDetails { get; set; }
    }
}
