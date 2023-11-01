using AutoMapper;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionListViewModel>()
                .ForMember(x => x.QuestionType, opt => opt.MapFrom(y => y.QuestionType.ToFriendlyString()))
                .ForMember(x => x.HotspotQuestionImage, opt => opt.MapFrom(y => y.HotspotQuestionImage.ImageUrl));

            CreateMap<Question, QuestionByIdViewModel>()
                .ForMember(x => x.ExistingImageName, opt => opt.MapFrom(y => y.HotspotQuestionImage.ImageUrl));

            // public
            CreateMap<Question, PublicQuestionViewModel>()
                 .ForMember(x => x.QuestionType, opt => opt.MapFrom(y => y.QuestionType.ToFriendlyString()));
        }
    }
}
