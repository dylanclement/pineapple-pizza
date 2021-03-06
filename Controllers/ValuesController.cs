﻿using System;
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
            var matchConfidence = 0F;
            MatchResponse response;
            try
            {
                string testPhotoId;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    // Fetch file content fomr base64 encoded body
                    var fileBase64 = await reader.ReadToEndAsync();
                    byte[] bytes = Convert.FromBase64String(fileBase64);

                    // Assign S3 object name and save to S3
                    testPhotoId = $"tmp_{Guid.NewGuid().ToString("D")}";
                    await _s3Connector.WriteObjectDataAsync(testPhotoId, bytes);
                }

                const int minThresholdPercentage = 85;

                //We extract the employee data from the id card
                (int employeeNumber, string employeeNameWithoutSpaces) employeeData = await _rekognitionConnector.ExtractNameAndCodeInS3Object(testPhotoId);

                var employee = await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId(employeeData.employeeNameWithoutSpaces, employeeData.employeeNumber));

                if (employee == null) throw new Exception("Employee not found");

                //we ensure that the faces are legit.
                matchConfidence = await _rekognitionConnector.FindFaceOrThrowException(employee.ActiveIdCard.PictureObjectId, testPhotoId, minThresholdPercentage, employee.ActiveIdCard.Name);

                response = new MatchResponse
                {
                    StatusCode = 200,
                    MatchConfidence = matchConfidence,
                    Message = "Welcome " + employee.ActiveIdCard.Name
                };
            }
            catch(Exception ex)
            {
                response = new MatchResponse
                {
                    StatusCode = 503,
                    Message = ex.Message
                };
            }

            // Return the response to the UI
            return Ok(response);
        }


        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var cardPhotoId = "213a8517-f372-4328-96c3-2134ab4501a8";
            var testPhotoId = "2ad44936-5441-4c00-89f3-9ec64e66d8e5";

            const int minThresholdPercentage = 80;

            //We extract the employee data from the id card
            (int employeeNumber, string employeeNameWithoutSpaces) employeeData = await _rekognitionConnector.ExtractNameAndCodeInS3Object(testPhotoId);

            var employee = await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId(employeeData.employeeNameWithoutSpaces, employeeData.employeeNumber));

            //we ensure that the faces are legit.
            //await _rekognitionConnector.FindFaceOrThrowException(employee.ActiveIdCard.PictureObjectId, testPhotoId, minThresholdPercentage);

            return Ok(
                //await _dynamoDBConnector.GetEmployeeAsync(new EmployeeId("BrunoTagliapietra", 9638)) + "\n" +
                //await _s3Connector.ReadObjectDataAsync("mytextfile.txt") + "\n" +
                //await _rekognitionConnector.DetectFaceInS3Object("213a8517-f372-4328-96c3-2134ab4501a8") + "\n" +
                //await _rekognitionConnector.CompareFacesInS3Objects("213a8517-f372-4328-96c3-2134ab4501a8", "a4778861-d477-466f-b7f9-745475de81d8", 70) + "\n" +
                //await _rekognitionConnector.DetectTextInS3Object("2ad44936-5441-4c00-89f3-9ec64e66d8e5") + "\n" +
                );
        }
    }
}
