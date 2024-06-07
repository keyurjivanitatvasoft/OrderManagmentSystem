using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.UserRepositry
{
    public interface IUserRepositry
    {
        Response Login(string username, string password);
    }
}
