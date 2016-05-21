using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace ResxDescriptionFilters.UsageExample
{
    [RoutePrefix("foos")]
    public class FooController : ApiController
    {
        public static IDictionary<int, Foo> _foos = new Dictionary<int, Foo>()
        {
            { 1, new Foo() { Thing = "Thiiing", OtherThing = 45 } },
            { 2, new Foo() { Thing = "Thng", OtherThing = 912 } }
        };

        [Route("")]
        [ResponseType(typeof(IEnumerable<Foo>))]
        public IHttpActionResult Get()
        {
            return Ok(_foos.Values);
        }

        [Route("")]
        [ResponseType(typeof(Foo))]
        public IHttpActionResult Post([FromBody]Foo value)
        {
            int id = _foos.Keys.Max() + 1;
            _foos[id] = value;
            return CreatedAtRoute("GetFoo", new { id = id }, value);
        }

        [Route("{id}", Name = "GetFoo")]
        [ResponseType(typeof(Foo))]
        public IHttpActionResult Get([FromUri]int id)
        {
            Foo foo = null;

            if (_foos.TryGetValue(id, out foo))
            {
                return Ok(foo);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
