using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Moduit.API.Controllers
{
    [Route("backend/question/one")]
    [ApiController]
    public class QuestionOneController : ControllerBase
    {        
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {     
            using (var httpClient = new HttpClient())
            {
                var jsonString = await httpClient.GetStringAsync("https://screening.moduit.id/backend/question/one");                
                dynamic json = JsonConvert.DeserializeObject(jsonString);

                // Now parse with JSON.Net
                return new JsonResult(json);

            }             
        }        
    }
}
