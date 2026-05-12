using Boi.Net.DTOs.UserDTOs;
using Boi.Net.Model;
using Microsoft.AspNetCore.Identity;

namespace Boi.Net.Services
{
    public class UserService
    {

        private readonly UserManager<User> _userManager;
        private readonly IPhotoService _photoService;

        public UserService(UserManager<User> userManager, IPhotoService photoService)
        {
            _userManager = userManager;
            _photoService = photoService;
        }


        public async Task<UserProfileResponseDto> MyProfileUpdateAsync(string userId, UpdateUserDto updateUserDto)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new Exception("User is not found.");
            }


            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                user.Name = updateUserDto.Name!;
            }


            if (updateUserDto.DOB.HasValue)
            {
                user.DOB = updateUserDto.DOB;
            }


            if(updateUserDto.ProfilePhoto != null)
            {

                if (!string.IsNullOrWhiteSpace(user.ProfilePhotoId))
                {
                    await _photoService.DeletePhotoAsync(user.ProfilePhotoId);
                }

                var photoUrl = await _photoService.AddPhotoAsync(updateUserDto.ProfilePhoto);
                if(photoUrl.Error != null)
                {
                    throw new Exception("Unable to upload image.");
                }

                user.ProfilePhotoUrl = photoUrl.SecureUrl.AbsoluteUri;
                user.ProfilePhotoId = photoUrl.PublicId;

            }


            user.UpdatedAt = DateTime.UtcNow;


            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Profile Update Failed.");
            }

            return new UserProfileResponseDto
            {
                Id = Guid.Parse(user.Id),
                Name = user.Name,
                Email = user.Email!,
                DOB = user.DOB,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                UserRole = user.UserRole
            };

        }



        public async Task<bool> SoftDeleteMyAccountAsync(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null || user.IsDeleted)
            {
                throw new Exception("User not found.");
            }

            user.IsDeleted = true;
            user.RefreshToken = null;
            user.UpdatedAt = DateTime.UtcNow;


            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;

        }


    }
}
