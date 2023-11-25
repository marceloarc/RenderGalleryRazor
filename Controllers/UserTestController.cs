using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Policy = "AdministradorOnly")]
    public class UserTestController : ControllerBase
    {
        private readonly DatabaseContext db;


        public UserTestController(DatabaseContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public List<User> Get()
        {

            List<User> users = new List<User>();

            users = db.Users.ToList();

            return users;
        }
    }
}