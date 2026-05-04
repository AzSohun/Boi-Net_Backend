using CloudinaryDotNet.Actions;

namespace Boi.Net.Services
{
    public interface IPhotoService
    {

        // Step 2: Create IPhoto Service for Uplaod and Delete Photo as Dreft. 
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);

    }
}
