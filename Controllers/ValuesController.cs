using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PineapplePizza.Model;

namespace PineapplePizza.Controllers
{
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

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId("BrunoTagliapietra", 9638)));
            //return Ok(await _connector.ReadObjectDataAsync("mytextfile.txt"));
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
