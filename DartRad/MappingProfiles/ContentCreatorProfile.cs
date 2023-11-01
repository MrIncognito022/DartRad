using AutoMapper;
using DartRad.Areas.Editor.Models;
using MimeKit.Cryptography;

namespace DartRad.MappingProfiles
{
    public class ContentCreatorProfile : Profile
    {
        public ContentCreatorProfile()
        {
            CreateMap<ContentCreator, ContentCreatorListViewModel>()
                .ForMember(x => x.InvitedByAdmin, opt => opt.MapFrom(y => y.InvitedByUser.Name))
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(y => y.CreatedAt.ToString("MMMM yyyy")));


            CreateMap<ContentCreator, ContentCreatorUpdateViewModel>();
        }
    }
}
