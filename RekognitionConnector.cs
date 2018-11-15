using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Options;
using PineapplePizza.Config;
using System.Text;

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

        public async Task<float> FindFaceOrThrowException(string s3ObjectIdCard, string s3ObjectPhotoToTest, float similarityThreshold)
        {
            var imgSrc = GetImageDefinition(s3ObjectIdCard);
            var imgTrg = GetImageDefinition(s3ObjectPhotoToTest);

            var compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imgSrc,
                TargetImage = imgTrg,
                SimilarityThreshold = similarityThreshold
            };

            try
            {
                var compareFacesResponse = await _client.CompareFacesAsync(compareFacesRequest);

                // we are looking for two faces, the card and the actual picture taken by the person.
                var matchesWeCareAbout = compareFacesResponse.FaceMatches.OrderByDescending(m => m.Similarity).Take(2);

                if (matchesWeCareAbout.Count() < 2) throw new Exception("Please ensure that both your card and your face can be seen");

                // the card should be the smallest photo of the two with highest similarity
                var shouldBeTheCardBySize = compareFacesResponse.FaceMatches.OrderBy(m => m.Face.BoundingBox.Height * m.Face.BoundingBox.Width).Take(1).Single();

                // the card should also be the one with the highest similarity
                var shouldBeTheCardBySimilarity = compareFacesResponse.FaceMatches.OrderByDescending(m => m.Similarity).Take(1).Single();

                if (shouldBeTheCardBySimilarity != shouldBeTheCardBySize) throw new Exception("Faces don't match");

                var shoultBeTheFaceOfThePerson = compareFacesResponse.FaceMatches.OrderBy(m => m.Similarity).Take(1).Single();

                return shoultBeTheFaceOfThePerson.Similarity;

                /*var match = shouldBeTheCardBySimilarity;
                var face = match.Face;
                var position = face.BoundingBox;
                System.Diagnostics.Debug.WriteLine("Face at " + position.Left + " " + position.Top + " matches with " + face.Confidence + "% confidence.");
                System.Diagnostics.Debug.WriteLine("There was " + compareFacesResponse.UnmatchedFaces.Count + " face(s) that did not match");
                System.Diagnostics.Debug.WriteLine("Source image rotation: " + compareFacesResponse.SourceImageOrientationCorrection);
                System.Diagnostics.Debug.WriteLine("Target image rotation: " + compareFacesResponse.TargetImageOrientationCorrection);*/
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<(int,string)> ExtractNameAndCodeInS3Object(string s3ObjectPhotoToTest)
        {
            var detectTextRequest = new DetectTextRequest()
            {
                Image = GetImageDefinition(s3ObjectPhotoToTest)
            };

            try
            {
                DetectTextResponse detectTextResponse = await _client.DetectTextAsync(detectTextRequest);
                System.Diagnostics.Debug.WriteLine("Detected lines and words for " + s3ObjectPhotoToTest);
                var uniqueStrings = new HashSet<string>();
                foreach (TextDetection text in detectTextResponse.TextDetections)
                {
                    System.Diagnostics.Debug.WriteLine("Detected: " + text.DetectedText);
                    System.Diagnostics.Debug.WriteLine("Confidence: " + text.Confidence);
                    System.Diagnostics.Debug.WriteLine("Id : " + text.Id);
                    System.Diagnostics.Debug.WriteLine("Parent Id: " + text.ParentId);
                    System.Diagnostics.Debug.WriteLine("Type: " + text.Type);
                }

                var textWeCareAbout = detectTextResponse.TextDetections.Where(td => td.Confidence > 90);

                if (textWeCareAbout.Count() < 2) throw new Exception("Please make your id card more visible");

                var uniqueTexts = textWeCareAbout.Select(td => td.DetectedText).ToHashSet();

                var probablyTheName = uniqueTexts.OrderByDescending(td => td.Length).Take(1).Single();
                var probablyTheEmployeeNumber = uniqueTexts.SingleOrDefault(td => td.All(chr => Char.IsNumber(chr)));

                if (probablyTheEmployeeNumber == null) throw new Exception("Couldn't read the employee number");

                int employeeNumber = int.Parse(probablyTheEmployeeNumber);
                string employeeNameWithoutSpaces = probablyTheName.Replace(" ", string.Empty);

                return (employeeNumber, employeeNameWithoutSpaces);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw e;
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
