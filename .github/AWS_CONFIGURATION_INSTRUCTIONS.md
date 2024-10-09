# AWS Configuration
## Private S3 Bucket with signed URLs in Cloudfront for Video Streaming:
1. Create an S3 bucket for logging (naming convention - <bucket name>-logs)
   a. Use default settings
1. Create an S3 Input Bucket to hold videos
   a. Use default settings
1. Edit the bucket – under “Server access logging” enable, and tie to the -logs bucket that was created in step 1.
1. Upload videos to S3 Input Bucket (with AWS console or AWS CLI - <https://aws.amazon.com/sdk-for-net/> )
1. Navigate to AWS IAM Service
    a. Create Policy to Set permissions to the S3 Bucket using IAM Roles (If adding videos from code, your IAM user needs role access to "PutObject" for the newly created S3 bucket.)
      1. Create policy
            ```json
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Sid": "VisualEditor0",
                            "Effect": "Allow",
                            "Action": "s3:PutObject",
                            "Resource": "arn:aws:s3:::<BUCKET NAME>/\*"
                        }
                    ]
                }
            ```
    b. Create new User Group – assign new policy to the group
    c. Create the IAM User and assign to the new group/policy just created. Save security credentials (Access key ID and Secret access key) for use in the website as docker/GitHub Action secrets
    
1. Set up Cloudfront distribution to deliver content (<http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/distribution-web.html>)
   a. Create distribution
   b. Set Origin Domain Name to the name of the S3 output bucket
   c. Set Origin Path if a folder of the S3 bucket is to be used (e.g. /dev, /stage, /prod)
   d. Set unique origin ID
   e. YES - Restrict Bucket Access (uses origin access identity)
   f. Grant Read Permissions on Bucket - can set to "Yes, Update Permissions" (then later, go back in and check S3 bucket's permissions for Cloudfront per 5.b below) à copy the policy from CloudFront and go back to S3 and go to the Permissions tab -> Bucket Permissions – and paste what CloudFront auto created (GetObject permissions for Cloudfront to that bucket)
   g. Viewer Protocol Policy - HTTPS Only
   h. Allowed HTTP Methods - GET, HEAD only
   i. Price Class (based on latency, only US, Canada, Europe is cheapest, per: <http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/PriceClass.html>)
   j. Distribution State - start disabled by default, so no requests are accepted yet.
1. Create Security Role using CloudFront Origin Access Identity to limit access only to the content of the S3 buckets (no direct access via S3 URLs - <http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/PrivateContent.html> and <http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/private-content-restricting-access-to-s3.html>)
   a. Add signed URLs or signed cookies to restrict communication between client and CloudFront
   b. Origin access identity restricts communication between CloudFront and S3
        i. In Cloudfront, click the "Origin Access Identity" link on the left to view users, and grab the Canonical User ID for the CloudFront user that should have access to the S3 bucket
        ii. In S3, go to your output bucket, and under Permissions, add the Canonical User ID from CloudFront as the user with read access. Deny all other public access.
   c. Update bucket policy allowing origin access to the bucket.
1. Add SSL Cert on the Cloudfront distribution. Note: this should be a free AWS cert
   a. SSL Certificate as "Default CloudFront Certificate" during setup (step 5 above)
1. If needing signed URLs - on the Distribution - under "Behaviors" - Edit - "Restrict Viewer Access" - check YES - then can set the "Trusted Signers"
   a. **Developer:** If needing to start videos at specific start/end times, once video player is tied into the new CloudFront distribution links. For Signed URLs, append the "#t=<start>,<end>" to the end of the URL after signing
   b. <https://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/private-content-trusted-signers.html?icmpid=docs_cf_help_panel#private-content-creating-cloudfront-key-pairs>

1. **Developer:** Set up web code to Stream videos with HLS, SS, or HDS - need HLS player (Video.js is open source - but documentation indicates it won't work with IE)
   a. In Visual Studio, install the AWSSDK from Nuget (<https://aws.amazon.com/sdk-for-net/>)
   b. Add the AWS SDK to your web.config (<http://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config.html>)
   c. Rebuild project
   d. Configure private key
      i. <http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/private-content-creating-signed-url-canned-policy.html>
1. Set up CloudWatch to monitor the health of the transcoding workflow
1. Test that all permissions are properly locked down. Set permission on the bucket containing the video files giving Read Bucket to the Cloudfront Account
- Content on S3 should not be accessible using the S3 URL
  - Check with the URL from uploading a test file to S3 and clicking on the Http link
- Content using the Cloudfront URL should be accessible via HTTPS
  - Try to get to the S3 file through CloudFront - <cloudfrontGUID.cloudfront.net/<filename>
- Content using the Cloudfront URL should NOT be accessible via HTTP
