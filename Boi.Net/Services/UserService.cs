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

        // 🚨 নতুন: ফ্রন্টএন্ডের GET রিকোয়েস্টের জন্য প্রোফাইল ফেচ করার মেথড
        public async Task<UserProfileResponseDto> GetMyProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted) throw new Exception("User not found or deleted.");

            return new UserProfileResponseDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email!,
                Name = user.Name,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                DOB = user.DOB,
                UserRole = user.UserRole
            };
        }

        public async Task<UserProfileResponseDto> MyProfileUpdateAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new Exception("User is not found.");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            if (updateUserDto.DOB.HasValue)
            {
                user.DOB = updateUserDto.DOB;
            }

            if (updateUserDto.ProfilePhoto != null)
            {
                if (!string.IsNullOrWhiteSpace(user.ProfilePhotoId))
                {
                    await _photoService.DeletePhotoAsync(user.ProfilePhotoId);
                }

                var photoUrl = await _photoService.AddPhotoAsync(updateUserDto.ProfilePhoto);
                if (photoUrl.Error != null)
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

            if (user == null || user.IsDeleted)
            {
                throw new Exception("User not found.");
            }

            user.IsDeleted = true;
            user.RefreshToken = null;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ManageUserByAdminAsync(string userId, AdminUpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new Exception("User not found.");
            }

            if (user.UserRole == Role.SuperAdmin)
            {
                throw new UnauthorizedAccessException("Super Admin role or status cannot be modified.");
            }

            user.UserRole = dto.UserRole;
            user.IsBlocked = dto.IsBlocked;

            if (dto.IsBlocked)
            {
                user.RefreshToken = null;
            }

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}