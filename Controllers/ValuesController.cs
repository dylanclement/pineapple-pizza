using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PineapplePizza.Model;

namespace PineapplePizza.Controllers
{
    public class Model 
    {
        public string imageFile;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        S3Connector _s3Connector;
        DynamoDBConnector _dynamoDBConnector;

        public ValuesController(S3Connector s3connector, DynamoDBConnector dynamoDBConnector)
        {
            _s3Connector = s3connector;
            _dynamoDBConnector = dynamoDBConnector;
        }

        // POST api/values
        [HttpPost]
        public async void Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                var fileBase64 = await reader.ReadToEndAsync();
                byte[] bytes = Convert.FromBase64String(fileBase64);

                // TODO: Save to S3
                //System.IO.File.WriteAllBytes("test.jpg", bytes);
                await _s3Connector.WriteObjectDataAsync("test", bytes);
            }
        }


        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId("BrunoTagliapietra", 9638)) +
                await _s3Connector.ReadObjectDataAsync("mytextfile.txt")
                );
        }
    }
}
