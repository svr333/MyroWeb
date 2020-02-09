using Microsoft.AspNetCore.Mvc;
using MyroWebClient;
using MyroWebClient.Entities;
using Newtonsoft.Json;

namespace MyroWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MyroDataService _myro;
        private readonly MyroDatabase _userDb;

        public UsersController(MyroDataService myro, MyroDatabase userDb)
        {
            _myro = myro;
            _userDb = userDb;
        }

        [HttpGet("{id}")]
        public Grades Get([FromRoute]string id)
        {
            User user = _userDb.GetUserByKey(id);
            return _myro.GetAllMyroData(user);
        }

        [HttpPost("{id}")]
        public void Get([FromRoute]string id, [FromBody]User user)
        {
            _userDb.StoreNewUser(id, user);
        }
    }
}
