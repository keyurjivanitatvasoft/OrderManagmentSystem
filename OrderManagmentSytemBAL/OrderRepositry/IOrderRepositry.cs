using OrderManagmentSytemDAL.DTOs;
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

        #region simple CRUD (Task 1)
        Response GetOrders();
        Response GetOrder(int orderId);
        Response CreateOrder(Order order);
        Response UpdateOrder(Order order);

        Response DeleteOrder(int orderId);
        #endregion

        #region CRUD SP (Task 2)
        Response SearchOrderSP(Order order);
        Response SaveOrdersSP(Order order,bool Isdelete);
        #endregion

        #region customerOrderDetails (Task 3)
        Response CustomerOrderDetails();
        #endregion

        #region Task 4

        Response OrdersExits(List<int> orderIds);
        Response DeleteOrders(List<int> orderIds);
        #endregion

        #region Api (Task 5)
        Response GetOrderById(int id);

        Response AddOrder(AddOrderDTO order);
        Response EditOrder(int id,AddOrderDTO order);
        #endregion
    }
}
