﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    [RoutePrefix("api")]
    public class HomeController : ApiController
    {
        [Route]
        public IHttpActionResult Get()
        {
            return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("this is documents api home")
            });
        }
    }
}
