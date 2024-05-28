using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.ViewModels
{
    public class Response
    {
        public HttpStatusCode StatusCode;
        public bool IsSuccess;
        public List<string> ErrorMessages;
        public object Result;
    }
}
