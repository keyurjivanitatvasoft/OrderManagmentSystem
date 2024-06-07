using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.Services.Auth;
using OrderManagmentSytemBAL.UserRepositry;
using OrderManagmentSytemDAL.DTOs;
using OrderManagmentSytemDAL.ViewModels;

namespace OrderManagmentSystem.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserRepositry userRepositry;
        private readonly IJWTService jWTService;
        public UserApiController(IUserRepositry userRepositry,IJWTService jWTService)
        {
            this.userRepositry = userRepositry;
            this.jWTService = jWTService;
        }
        [HttpPost]
        public IActionResult LoginUser(LoginRequest loginRequest)
        {
            Response userResponse = userRepositry.Login(loginRequest.UserName, loginRequest.Password);
            if(userResponse.IsSuccess &&  userResponse.Result is IEnumerable<User> user && user.Count()==1)
            {
                string jwtToken = jWTService.GenerateJwtToken(user.First());
                return Ok(new {message ="user login successfully",Token=jwtToken});
            }
            return BadRequest("UserName or Password was invalid.");
        }
    }
}
