using AsnParser.Models;
using AsnParser.Models.Entities;
using AutoMapper;

namespace AsnParser.MapperProfiles;

public class BoxProfile : Profile
{
    public BoxProfile()
    {
        CreateMap<Content, ContentEntity>();
        CreateMap<Box, BoxEntity>().ForMember(dst => dst.Id, x => x.Ignore());
    }
}