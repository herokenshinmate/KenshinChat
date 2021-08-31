using KenshinChat.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenshinChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private string _UserName = "Garion";
        private string _Password = "password";
        
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody]User user)
        {
            if (user.UserName == _UserName && user.Password == _Password)
                return NoContent();
            else
                return Unauthorized();
        }
    }
}
