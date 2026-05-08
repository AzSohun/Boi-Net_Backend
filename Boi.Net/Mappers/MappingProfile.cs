using AutoMapper;
using Boi.Net.DTOs.AuthDTOs;
using Boi.Net.DTOs.BookDTOs;
using Boi.Net.DTOs.UserDTOs;
using Boi.Net.Model;

namespace Boi.Net.Mappers
{
    public class MappingProfile: Profile
    {

        public MappingProfile()
        {

            // BookDto Mapping
            CreateMap<BookDto, Book>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();


            // AuthDto Mapping
            CreateMap<LoginDto, User>();
            CreateMap<RegistrationDto, User>();


            // UserDto Mapping
            CreateMap<UserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<UserProfileResponseDto, User>();

        }

    }
}
