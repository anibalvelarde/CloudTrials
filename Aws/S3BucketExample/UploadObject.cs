using System;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3BucketExample
{
    /// <summary>
    /// Example that shows how to upload files to an AWS S3 Bucket
    /// </summary>
    class UploadObject
    {
        static string bucketName = @"Type-It-In";
        static string keyName = "BusinessModelCanvas";
        static string filePath = @"..\..\..\..\UploadFiles\BMC.pdf";
        static Amazon.RegionEndpoint awsRegion = Amazon.RegionEndpoint.USEast1; 

        static IAmazonS3 client;

        public static void Main(string[] args)
        {
            CreateProfile();

            GetBucketName();

            using (client = new AmazonS3Client(awsRegion))
            {
                Console.WriteLine("Uploading an object");
                WritingAnObject();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void GetBucketName()
        {
            Console.Write("Enter AWS Bucket Name ---> ");
            bucketName = Console.ReadLine();
        }

        /// <summary>
        /// This creates an AWS user profile entry in the file located in this 
        /// folder: C:\Users\<username>\AppData\Local\AWSToolkit\RegisteredAccounts.json
        /// 
        /// See reference at https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html
        /// 
        /// This is done for expediency, experiment and example purposes. For real production functionality you do
        /// not want to expose your credentials in this manner. For security and credential file concerns
        /// see this reference: https://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/net-dg-config-creds.html#creds-assign
        /// 
        /// </summary>
        private static void CreateProfile()
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = "add-user-key-here",
                SecretKey = "add-user-secret-here"
            };
            var profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("basic_profile", options);
            profile.Region = awsRegion;
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);
        }

        static void WritingAnObject()
        {
            try
            {
                PutObjectRequest putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    ContentBody = "sample text"
                };

                PutObjectResponse response1 = client.PutObject(putRequest1);

                // 2. Put object-set ContentType and add metadata.
                PutObjectRequest putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filePath,
                    ContentType = "application/pdf"
                };
                putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");

                PutObjectResponse response2 = client.PutObject(putRequest2);

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                        "For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                        "Error occurred. Message:'{0}' when writing an object"
                        , amazonS3Exception.Message);
                }
            }
        }
    }
}