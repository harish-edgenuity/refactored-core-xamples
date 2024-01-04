using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace S3.Demo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly AmazonS3Client s3client;
        public BucketsController()
        {
            s3client = new AmazonS3Client("AWS Access key",
                            "AWS Secret access key",
                            RegionEndpoint.GetBySystemName("ap-south-1"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3client, bucketName);
            if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");
            await s3client.PutBucketAsync(bucketName);
            return Created("buckets", $"Bucket {bucketName} created.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBucketAsync()
        {
            var data = await s3client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            await s3client.DeleteBucketAsync(bucketName.Trim());
            return NoContent();
        }
    }
}
 
