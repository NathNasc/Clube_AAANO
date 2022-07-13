using System;
using System.Net.Http;
using System.Web.Mvc;

namespace ClubeAaanoSite.Controllers
{
    [RequireHttps]
    public class BaseController : Controller
    {
        internal HttpClient client { get; set; }

        public BaseController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://admclubeaaano.com.br");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}