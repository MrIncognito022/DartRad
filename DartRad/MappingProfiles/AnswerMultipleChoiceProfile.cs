using AutoMapper;
using DartRad.Areas.ContentCreator.Models;
using DartRad.ViewModels;

namespace DartRad.MappingProfiles
{
    public class AnswerMultipleChoiceProfile: Profile
    {
        public AnswerMultipleChoiceProfile()
        {
            CreateMap<AnswerMultipleChoice, AnswerViewModel>();
            CreateMap<AnswerMultipleChoice, PublicAnswerMultipleChoiceViewModel>();

        }
    }
}
