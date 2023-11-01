using AutoMapper;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class AnswerSequenceProfile : Profile
    {
        public AnswerSequenceProfile()
        {
            CreateMap<AnswerSequence, AnswerSequenceViewModel>();
            CreateMap<AnswerSequence, PublicAnswerSequenceViewModel>();
        }
    }
}
