using AutoMapper;
using Boi.Net.DTOs.BookDTOs;
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

        }

    }
}
