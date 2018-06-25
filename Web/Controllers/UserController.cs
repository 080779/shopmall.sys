using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class UserController : ApiController
    {
        [HttpPost]
        public IEnumerable<string> Get(Person p)
        {
            return new string[] { p.Name, p.Age.ToString() };
        } 
    }
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}