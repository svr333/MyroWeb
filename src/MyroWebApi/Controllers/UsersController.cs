using Microsoft.AspNetCore.Mvc;
using MyroWebClient;
using MyroWebClient.Entities;

namespace MyroWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MyroDataService _myro;
        private readonly MyroDatabase _userDb;

        public UsersController(MyroDataService myro, MyroDatabase userDb)
        {
            _myro = myro;
            _userDb = userDb;
        }

        [HttpGet]
        public Grades Get()
        {
            return _myro.GetAllMyroData(new User { UserName = "USERNAME", Password = "PASSWORD", SchoolAbreviation = "SCHOOLABREVIATION" });
        }

        [HttpGet]
        public Grades Get(string id)
        {
            User user = _userDb.GetUserByKey(id);
            return _myro.GetAllMyroData(user);
        }
    }
}
