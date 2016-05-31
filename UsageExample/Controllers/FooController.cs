namespace Swashbuckle.DynamicLocalization.UsageExample
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    [RoutePrefix("foos")]
    public class FooController : ApiController
    {
        public static IDictionary<int, Foo> _foos = new Dictionary<int, Foo>()
        {
            { 1, new Foo() { Thing = "Thiiing", OtherThing = 45 } },
            { 2, new Foo() { Thing = "Thang", OtherThing = 912 } }
        };

        [Route("")]
        [ResponseType(typeof(IEnumerable<Foo>))]
        public IHttpActionResult Get()
        {
            return Ok(_foos.Values);
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

        [Route("")]
        [ResponseType(typeof(Foo))]
        public IHttpActionResult Post([FromBody]Foo value)
        {
            int newId = _foos.Keys.Max() + 1;
            _foos[newId] = value;
            return CreatedAtRoute("GetFoo", new { id = newId }, value);
        }
    }
}
