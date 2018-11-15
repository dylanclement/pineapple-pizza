using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Options;
using PineapplePizza.Config;

namespace PineapplePizza
{
    public class RekognitionConnector
    {
        private IAmazonRekognition _client;
        private string _s3BucketName;

        public RekognitionConnector(IOptions<AppConfig> appConfigOptions, IAmazonRekognition client) :
            this(appConfigOptions.Value.S3BucketName, client)
        {
        }

        public RekognitionConnector(string s3BucketName, IAmazonRekognition client)
        {
            _client = client;
            _s3BucketName = s3BucketName;
        }

        public async Task<float> DetectFaceInS3Object(string s3ObjectName)
        {
            var detectFacesRequest = new DetectFacesRequest()
            {
                Image = GetImageDefinition(s3ObjectName),
                // Attributes can be "ALL" or "DEFAULT". 
                // "DEFAULT": BoundingBox, Confidence, Landmarks, Pose, and Quality.
                // "ALL": See https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Rekognition/TFaceDetail.html
                Attributes = new List<String>() { "ALL" }
            };

            try
            {
                var detectFacesResponse = await _client.DetectFacesAsync(detectFacesRequest);
                bool hasAll = detectFacesRequest.Attributes.Contains("ALL");

                var face = detectFacesResponse.FaceDetails.SingleOrDefault();
                if (face == null) return 0;

                System.Diagnostics.Debug.WriteLine("BoundingBox: top={0} left={1} width={2} height={3}", face.BoundingBox.Left, face.BoundingBox.Top, face.BoundingBox.Width, face.BoundingBox.Height);
                System.Diagnostics.Debug.WriteLine("Confidence: {0}\nLandmarks: {1}\nPose: pitch={2} roll={3} yaw={4}\nQuality: {5}", face.Confidence, face.Landmarks.Count, face.Pose.Pitch, face.Pose.Roll, face.Pose.Yaw, face.Quality);

                if (hasAll) System.Diagnostics.Debug.WriteLine("The detected face is estimated to be between " + face.AgeRange.Low + " and " + face.AgeRange.High + " years old.");

                return face.Confidence;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<float> CompareFacesInS3Objects(string s3ObjectSourceName, string s3ObjectTargetName, float similarityThreshold)
        {
            var imgSrc = GetImageDefinition(s3ObjectSourceName);
            var imgTrg = GetImageDefinition(s3ObjectTargetName);

            var compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imgSrc,
                TargetImage = imgTrg,
                SimilarityThreshold = similarityThreshold
            };

            try
            {
                var compareFacesResponse = await _client.CompareFacesAsync(compareFacesRequest);

                // Display results
                var match = compareFacesResponse.FaceMatches.SingleOrDefault();
                if (match == null) return 0;
                var face = match.Face;
                var position = face.BoundingBox;
                System.Diagnostics.Debug.WriteLine("Face at " + position.Left + " " + position.Top + " matches with " + face.Confidence + "% confidence.");
                System.Diagnostics.Debug.WriteLine("There was " + compareFacesResponse.UnmatchedFaces.Count + " face(s) that did not match");
                System.Diagnostics.Debug.WriteLine("Source image rotation: " + compareFacesResponse.SourceImageOrientationCorrection);
                System.Diagnostics.Debug.WriteLine("Target image rotation: " + compareFacesResponse.TargetImageOrientationCorrection);

                return face.Confidence;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        private Image GetImageDefinition(string s3ObjectName)
        {
            return new Image()
            {
                S3Object = new S3Object()
                {
                    Name = s3ObjectName,
                    Bucket = _s3BucketName
                },
            };
        }
    }
}
