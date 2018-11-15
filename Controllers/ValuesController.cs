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
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        S3Connector _s3Connector;
        DynamoDBConnector _dynamoDBConnector;
        RekognitionConnector _rekognitionConnector;

        public ValuesController(S3Connector s3connector, DynamoDBConnector dynamoDBConnector, RekognitionConnector rekognitionConnector)
        {
            _s3Connector = s3connector;
            _dynamoDBConnector = dynamoDBConnector;
            _rekognitionConnector = rekognitionConnector;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                // Fetch file content fomr base64 encoded body
                var fileBase64 = await reader.ReadToEndAsync();
                byte[] bytes = Convert.FromBase64String(fileBase64);

                // Assign S3 object name and save to S3
                var picName = Guid.NewGuid().ToString("D");
                await _s3Connector.WriteObjectDataAsync(picName, bytes);

                // Return the response to the UI
                var response = new MatchResponse 
                {
                    // TODO! Fetch values from rekognition
                    StatusCode = 200,
                    MatchConfidence = 92.39f
                };
                return Ok(response);
            }
        }


        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId("BrunoTagliapietra", 9638)) + "\n" +
                await _s3Connector.ReadObjectDataAsync("mytextfile.txt") + "\n" +
                await _rekognitionConnector.DetectFaceInS3Object("213a8517-f372-4328-96c3-2134ab4501a8") + "\n" +
                await _rekognitionConnector.CompareFacesInS3Objects("213a8517-f372-4328-96c3-2134ab4501a8", "a4778861-d477-466f-b7f9-745475de81d8", 70)
                );
        }
    }
}
