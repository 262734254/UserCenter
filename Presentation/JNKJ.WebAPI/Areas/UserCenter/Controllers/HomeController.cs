using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("Hello World");
            return response;
        }
    }
}
