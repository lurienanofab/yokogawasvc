using System.Web.Http;

namespace YokogawaService.Controllers
{
    public class DefaultController : ApiController
    {
        [Route("")]
        public string Get()
        {
            return "yokogawasvc";
        }
    }
}
