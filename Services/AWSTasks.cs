using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Security.AccessControl;

namespace ObserveAssign.Services
{
    public class AWSTasks
    {
        /// <summary>
        /// Shows how to upload a file from the local computer to an Amazon S3 bucket.
        /// From https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/csharp_s3_code_examples.html
        /// </summary>
        /// <param name="client">An initialized Amazon S3 client object.</param>
        /// <param name="bucketName">The Amazon S3 bucket to which the object will be uploaded.</param>
        /// <param name="objectName">The object to upload.</param>
        /// <param name="filePath">The path, including file name, of the object on the local computer to upload.</param>
        /// <returns>A boolean value indicating the success or failure of the upload procedure.</returns>
        public static async Task<bool> UploadFileAsync(IAmazonS3 client, string bucketName, string objectName, string filePath)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filePath,
            };

            var response = await client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }

        /// <summary>
        /// Used for CloudFront when using signed URLs
        /// </summary>
        /// <returns></returns>
        //public string CloudFrontPrivate(string videoS3Url)
        //{
        //    //int startTimeSecs = 10; //the number of seconds into the video that should start playing
        //    //int endTimeSecs = 20; //the end time of the video snippet to play
        //    //string videoS3Url = "https://<guid>.cloudfront.net/test-video-private.mp4"; //index.m3u8"; 

        //    string signedURL = CloudFrontStreaming.getSignedURL(videoS3Url);
        //    //if you need to change the start and end time
        //    //string URL = String.Concat(signedURL, "#t=",startTimeSecs,",",endTimeSecs);
        //    //model.URL = signedURL;
        //    //If needed to handle formats
        //    //model.SourceType = "video/mp4"; //video/mp4 = MP4 format
        //                                    //"application/x-mpegURL"; //HLS format

        //    return signedURL;
        //}


        /// <summary>
        /// Uploads a file to S3 using an IAM user with permission to PutObject to the S3 bucket
        /// </summary>
        /// <param name="model">The model storing the values the user provides for the file and file name</param>
        /// <returns></returns>
        public static string UploadToS3(IFormFile userPostedFile, string FileName, IConfiguration configuration)
        {
            //IAM Access key for service user
            string accessID = configuration["AWSS3AccessID"];
            string privateAccessKey = configuration["AWSS3PrivateAccessKey"];
            string _bucketName = configuration["AWSS3BucketName"];

            //connect to S3
            using (AmazonS3Client client = new AmazonS3Client(accessID, privateAccessKey, Amazon.RegionEndpoint.USEast1))
            {
                //upload to bucket
                try
                {
                    string filePath = userPostedFile.FileName.Substring(userPostedFile.FileName.LastIndexOf('.'));

                    //if user provided file name doesn't include the path
                    if (!FileName.EndsWith(filePath))
                    {
                        FileName = FileName + filePath; //append the path
                    }

                    ////use transfer utility for larger files
                    //var transferUtil = new TransferUtility(client);
                    //transferUtil.Upload(filePath, _bucketName, FileName);
                    ////var success = await TransferMethods.UploadSingleFileAsync(transferUtil, _bucketName, fileToUpload, localPath);

                    // Put object, set ContentType and add metadata.
                    PutObjectRequest putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = FileName,
                    };

                    putRequest.InputStream = userPostedFile.OpenReadStream(); //.InputStream;
                    //put Object in S3
                    PutObjectResponse putResponse = client.PutObjectAsync(putRequest).Result;

                    if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Successfully uploaded {FileName}.");
                        return FileName;
                    }
                    else
                    {
                        Console.WriteLine($"Could not upload {FileName}.");
                        return "Error";
                    }
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Console.WriteLine("Check the provided AWS Credentials.");
                        Console.WriteLine("For service sign up go to http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Console.WriteLine("Error occurred. Message:'{0}' when writing an object", amazonS3Exception.Message);
                    }
                }
            }

            return FileName;
        }

        /// <summary>
        /// Uploads a file to S3 using an IAM user with permission to PutObject to the S3 bucket
        /// </summary>
        /// <param name="model">The model storing the values the user provides for the file and file name</param>
        /// <returns></returns>
        public static string UploadToS3FromLocalFile(string tempLocalPostedFile, string userProvidedFileName, IConfiguration configuration)
        {
            //IAM Access key for service user
            string accessID = configuration["AWSS3AccessID"];
            string privateAccessKey = configuration["AWSS3PrivateAccessKey"];
            string _bucketName = configuration["AWSS3BucketName"];

            //connect to S3
            using (AmazonS3Client client = new AmazonS3Client(accessID, privateAccessKey, Amazon.RegionEndpoint.USEast1))
            {
                //upload to bucket
                try
                {
                    // Put object, set ContentType and add metadata.
                    PutObjectRequest putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = userProvidedFileName,
                    };

                    putRequest.InputStream = File.OpenRead(tempLocalPostedFile);
                    //put Object in S3
                    PutObjectResponse putResponse = client.PutObjectAsync(putRequest).Result;

                    if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Successfully uploaded {userProvidedFileName}.");
                        return userProvidedFileName;
                    }
                    else
                    {
                        Console.WriteLine($"Could not upload {userProvidedFileName}.");
                        return "Error";
                    }
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Console.WriteLine("Check the provided AWS Credentials.");
                        Console.WriteLine("For service sign up go to http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Console.WriteLine("Error occurred. Message:'{0}' when writing an object", amazonS3Exception.Message);
                    }
                }
            }

            return userProvidedFileName;
        }

        public static string UploadMultiPartS3(IFormFile userPostedFile, string FileName, IConfiguration configuration)
        {
            //IAM Access key for service user
            string accessID = configuration["AWSS3AccessID"];
            string privateAccessKey = configuration["AWSS3PrivateAccessKey"];
            string _bucketName = configuration["AWSS3BucketName"];

            using (TransferUtility transferUtil = new TransferUtility(accessID, privateAccessKey, Amazon.RegionEndpoint.USEast1))
            {
                //upload to bucket
                try
                {
                    string filePath = userPostedFile.FileName.Substring(userPostedFile.FileName.LastIndexOf('.'));
                    CancellationToken cancellationToken = new CancellationToken();
                    

                    //if user provided file name doesn't include the path
                    if (!FileName.EndsWith(filePath))
                    {
                        FileName = FileName + filePath; //append the path
                    }

                    //use transfer utility for larger files - give it 3 minutes to complete
                    transferUtil.UploadAsync(userPostedFile.OpenReadStream(), _bucketName, FileName, cancellationToken).Wait(180000);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occurred uploading to S3. '{0}'", e.Message);
                }
            }

            return FileName;
        }
    }
}
