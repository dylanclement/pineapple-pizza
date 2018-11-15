using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using PineapplePizza.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza
{
    public class S3Connector
    {
        private IAmazonS3 _client;
        private string _s3BucketName;

        public S3Connector(IOptions<AppConfig> appConfigOptions, IAmazonS3 client) :
            this(appConfigOptions.Value.S3BucketName, client)
        {
        }

        public S3Connector(string s3BucketName, IAmazonS3 client)
        {
            _client = client;
            _s3BucketName = s3BucketName;
        }

        public async Task<string> ReadObjectDataAsync(string objectKeyName)
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _s3BucketName,
                    Key = objectKeyName
                };
                using (GetObjectResponse response = await _client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    System.Diagnostics.Debug.WriteLine("Object metadata, Title: {0}", title);
                    System.Diagnostics.Debug.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                    return responseBody;
                }
            }
            catch (AmazonS3Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                throw;
            }
        }
    }
}
