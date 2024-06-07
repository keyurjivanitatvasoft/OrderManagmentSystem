using Dapper;
using Microsoft.Extensions.Options;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.DTOs;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.UserRepositry
{
    public class UserRespositry : IUserRepositry
    {
        private readonly OrderManagmentSystemDapperContext context;
        public UserRespositry(IOptions<ConnectionStrings> connectionStrings)
        {
            context = new OrderManagmentSystemDapperContext(connectionStrings.Value.OrderManagmentSystem);
        }
        public Response Login(string username, string password)
        {
            Response response = new Response();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", username);
                parameters.Add("@Password", password);

                IEnumerable<User> customersData = context.Query<User>("LoginUser", parameters, true);
                if (customersData.Count() == 1)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Result = customersData;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode=HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
    }
}
