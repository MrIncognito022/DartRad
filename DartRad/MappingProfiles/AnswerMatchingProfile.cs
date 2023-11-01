using AutoMapper;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class AnswerMatchingProfile: Profile
    {
        public AnswerMatchingProfile()
        {
            CreateMap<AnswerMatching, AnswerMatchingViewModel>();
            CreateMap<AnswerMatching, PublicMatchingAnswerViewModel>();
        }
    }
}
