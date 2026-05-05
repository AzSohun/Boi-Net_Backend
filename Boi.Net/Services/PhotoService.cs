using Boi.Net.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Boi.Net.Services
{
    public class PhotoService: IPhotoService
    {

        // Step 2:
        // Dependency Injection
        private readonly Cloudinary _cloudinary;


        // PhotoService Constructor
        public PhotoService(IOptions<CloudinarySettings> config)
        {

            // Get All The Value From AppSettiongs using IOptions and Inject into the account.
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);


            _cloudinary = new Cloudinary(account);
        }


        // It Uploads the Photo into Cloudinary
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                    // This piece of code will adjust the size before upload it to the Cloudinary.
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face"),

                    Folder = "BoiNet" // This is the Folder name where the image will be saved in the cloudinary.
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }

            return uploadResult;
        }


        // It Deletes the Photo from Cloudinary
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
