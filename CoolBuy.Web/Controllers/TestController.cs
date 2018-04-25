using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoolBuy.Business;

namespace CoolBuy.Web.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        //[HttpGet]
        //public int Test()
        //{
        //    return 1;
        //}

        [HttpGet]
        public string GetName()
        {
            //return "aa";
            return new Test().GetName();
        }
    }
}
