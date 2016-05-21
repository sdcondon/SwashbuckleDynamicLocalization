using System.Web.Http;

namespace ResxDescriptionFilters.Tests
{
    class DocumentedController : ApiController
    {
        [HttpGet]
        public string Get(int id)
        {
            return string.Empty;
        }
    }
}
