namespace Swashbuckle.DynamicLocalization.Tests
{
    using System.Web.Http;

    class DocumentedController : ApiController
    {
        [HttpGet]
        public string Get(int id)
        {
            return string.Empty;
        }
    }
}
