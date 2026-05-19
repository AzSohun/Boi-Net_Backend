using Boi.Net.DTOs.BookDTOs;
using Boi.Net.DTOs.UserDTOs;
using Boi.Net.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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


        public async Task<UserDto[]> GetAllProfile()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(user => new UserDto
                {
                    id = Guid.Parse(user.Id),
                    Email = user.Email!,
                    Name = user.Name,
                    ProfilePhotoUrl = user.ProfilePhotoUrl ?? string.Empty,
                    ProfilePhotoId = user.ProfilePhotoId ?? string.Empty,
                    DOB = user.DOB,
                    UserRole = user.UserRole,

                    Wishlist = user.Wishlist != null
                        ? user.Wishlist.Select(b => new BookDto
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            Price = b.Price,
                            CoverPhoto = b.CoverPhoto

                        }).ToList()
                        : new List<BookDto>()
                })
                .ToArrayAsync();

            return users;
        }


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

        // All User Deletation (Test Purpose) - Let it stay here
        public async Task<bool> HardClearUserTableAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            if (allUsers.Count == 0) return false;

            foreach (var user in allUsers)
            {
                await _userManager.DeleteAsync(user);
            }

            return true;
        }
    }
}