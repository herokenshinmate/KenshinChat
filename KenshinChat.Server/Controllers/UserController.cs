using KenshinChat.Server.Auth;
using KenshinChat.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenshinChat.Server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private ApiDbContext _db;

        public UserController()
        {
            _db = new ApiDbContext();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]User user)
        {
            //Checks the data validation and returns bad request if its invalid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Checks details
            User _user = _db.Users.FirstOrDefault(
                u => u.Username == user.Username && u.Password == user.Password);
            if (_user == null)
                return new UnauthorizedResult();

            //Create the token
            string accessToken = JWTAuth.GenerateToken(_user);
            return new ObjectResult(new
            {
                UserId = _user.UserId,
                ProfilePicture = _user.ProfilePicture,
                _user.Username,
                AccessToken = accessToken
            });
        }

        [HttpPost("register")]
        public IActionResult CreateUser([FromBody]User user)
        {
            //Checks the data validation and returns bad request if its invalid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Checks to see if a user already exists
            List<User> Users = _db.Users;
            if (Users.FirstOrDefault(u => u.Username == user.Username) != null)
                return new UnauthorizedResult();

            //Auto increment Id
            if (Users.Count == 0)
                user.UserId = 1;
            else
                user.UserId = Users.Last().UserId + 1;

            _db.Users.Add(user);
            _db.SaveChanges();

            //Create the token
            string accessToken = JWTAuth.GenerateToken(user);
            return new ObjectResult(new
            {
                user.UserId,
                ProfilePicture = user.ProfilePicture,
                user.Username,
                AccessToken = accessToken
            });
        }

        [HttpPost("GetProfilePicture")]
        public IActionResult GetProfilePicture([FromBody]int UserId)
        {
            byte[] img = _db.Users.FirstOrDefault(u => u.UserId == UserId).ProfilePicture;

            return new ObjectResult(new
            {
                ProfilePicture = img
            });
        }
    }
}
