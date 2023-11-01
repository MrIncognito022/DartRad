using AutoMapper;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class AnswerShortAnswerProfile: Profile
    {
        public AnswerShortAnswerProfile()
        {
            CreateMap<AnswerShortAnswer, AnswerViewModel>();
            CreateMap<AnswerShortAnswer, PublicAnsweShortAnswerViewModel>();

        }
    }
}
