using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.OrderRepositry
{
    public  interface IOrderRepositry
    {
        Response GetOrders();
        Response GetOrder(int orderId);
        Response CreateOrder(Order order);
        Response UpdateOrder(Order order);

        Response DeleteOrder(int orderId);

       
    }
}
