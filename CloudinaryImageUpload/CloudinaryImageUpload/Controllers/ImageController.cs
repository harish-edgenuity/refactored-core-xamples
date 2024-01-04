using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CloudinaryImageUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        public ImageController()
        {
            Account account = new Account("cloudinary",  //CloudName 
               " ",                      //my_api_key or Access Keys
               " "  //-api-secret
               );

            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile PostPicture)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(PostPicture.FileName, PostPicture.OpenReadStream()),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            //string filename = Path.GetFileName(uploadResult.Uri.AbsoluteUri);

            return Ok(uploadResult.StatusCode);
        }

        [HttpDelete]
            public async Task<IActionResult> Delete(string publicId)
        {
             if(string.IsNullOrEmpty(publicId))
            {
                return BadRequest();
            }
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };
            
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "ok")
            {
                //do action
                return Ok(deletionResult.Result);
            }
            else
            {
                return BadRequest(deletionResult.Result);
            }
           
        }

    }
}
